using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Linker.Tests.TestCases;
using Mono.Linker.Tests.Extensions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.TestCasesRunner {
	public class TestCaseCollector {
		private readonly NPath _rootDirectory;
		private readonly NPath _testCaseAssemblyPath;
		private readonly NPath _fsharpRootDirectory;
		private readonly NPath _fsharpTestCaseAssemblyPath;

		public TestCaseCollector (string rootDirectory, string testCaseAssemblyPath, string fsharpRootDirectory, string fsharpTestCaseAssemblyPath)
			: this (rootDirectory.ToNPath (), testCaseAssemblyPath.ToNPath (), fsharpRootDirectory.ToNPath (), fsharpTestCaseAssemblyPath.ToNPath ())
		{
		}

		public TestCaseCollector (NPath rootDirectory, NPath testCaseAssemblyPath, NPath fsharpRootDirectory, NPath fsharpTestCaseAssemblyPath)
		{
			_rootDirectory = rootDirectory;
			_testCaseAssemblyPath = testCaseAssemblyPath;
			_fsharpRootDirectory = fsharpRootDirectory;
			_fsharpTestCaseAssemblyPath = fsharpTestCaseAssemblyPath;
		}

		public IEnumerable<TestCase> Collect ()
		{
			return Collect (AllSourceFiles ()).Concat (CollectFSharp (AllSourceFilesFSharp ().ToArray()));
		}

		public TestCase Collect (NPath sourceFile)
		{
			return Collect (new [] { sourceFile }).First ();
		}

		private IEnumerable<TestCase> Collect (IEnumerable<NPath> sourceFiles)
		{
			_rootDirectory.DirectoryMustExist ();
			_testCaseAssemblyPath.FileMustExist ();

			using (var caseAssemblyDefinition = AssemblyDefinition.ReadAssembly (_testCaseAssemblyPath.ToString ())) {
				foreach (var file in sourceFiles) {
					TestCase testCase;
					if (CreateCase (caseAssemblyDefinition, file, _rootDirectory, _testCaseAssemblyPath, out testCase))
						yield return testCase;
				}
			}
		}
		
		private IEnumerable<TestCase> CollectFSharp (IEnumerable<NPath> sourceFiles)
		{
			_fsharpRootDirectory.DirectoryMustExist ();
			_fsharpTestCaseAssemblyPath.FileMustExist ();

			using (var caseAssemblyDefinition = AssemblyDefinition.ReadAssembly (_fsharpTestCaseAssemblyPath.ToString ())) {
				foreach (var file in sourceFiles) {
					TestCase testCase;
					if (CreateCase (caseAssemblyDefinition, file, _fsharpRootDirectory, _fsharpTestCaseAssemblyPath, out testCase))
						yield return testCase;
				}
			}
		}

		public IEnumerable<NPath> AllSourceFiles ()
		{
			return AllSourceFilesIn(_rootDirectory);
		}
		
		public IEnumerable<NPath> AllSourceFilesFSharp ()
		{
			return AllSourceFilesIn(_fsharpRootDirectory);
		}

		private static IEnumerable<NPath> AllSourceFilesIn(NPath directory)
		{
			directory.DirectoryMustExist ();

			foreach (var file in directory.Files ().Where (IsSourceFile)) {
				yield return file;
			}

			foreach (var subDir in directory.Directories ()) {
				if (subDir.FileName == "bin" || subDir.FileName == "obj" || subDir.FileName == "Properties")
					continue;

				foreach (var file in subDir.Files (true).Where (IsSourceFile)) {

					// Magic : Anything in a directory named Dependnecies is assumed to be a dependency to a test case
					// and never a test itself
					// This makes life a little easier when writing these supporting files as it removes some contraints you would previously have
					// had to follow such as ensuring a class exists that matches the file name and putting [NotATestCase] on that class
					if (file.Parent.FileName == "Dependencies")
						continue;

					// Magic: Anything in a directory named Individual is expected to be ran by it's own [Test] rather than as part of [TestCaseSource]
					if (file.Parent.FileName == "Individual")
						continue;

					yield return file;
				}
			}
		}

		private static bool IsSourceFile (NPath path)
		{
			return path.ExtensionWithDot == ".cs" || path.ExtensionWithDot == ".fs";
		}

		public TestCase CreateIndividualCase (Type testCaseType)
		{
			_rootDirectory.DirectoryMustExist ();
			_testCaseAssemblyPath.FileMustExist ();

			var pathRelativeToAssembly = $"{testCaseType.FullName.Substring (testCaseType.Module.Name.Length - 3).Replace ('.', '/')}.cs";
			var fullSourcePath = _rootDirectory.Combine (pathRelativeToAssembly).FileMustExist ();

			using (var caseAssemblyDefinition = AssemblyDefinition.ReadAssembly (_testCaseAssemblyPath.ToString ()))
			{
				TestCase testCase;
				if (!CreateCase (caseAssemblyDefinition, fullSourcePath, _rootDirectory, _testCaseAssemblyPath, out testCase))
					throw new ArgumentException ($"Could not create a test case for `{testCaseType}`.  Ensure the namespace matches it's location on disk");

				return testCase;
			}
		}

		private static bool CreateCase (AssemblyDefinition caseAssemblyDefinition, NPath sourceFile, NPath rootDirectory, NPath testCaseAssemblyPath, out TestCase testCase)
		{
			var potentialCase = new TestCase (sourceFile, rootDirectory, testCaseAssemblyPath);

			var typeDefinition = FindTypeDefinition (caseAssemblyDefinition, potentialCase);

			testCase = null;

			if (typeDefinition == null) {
				Console.WriteLine ($"Could not find the matching type for test case {sourceFile}.  Ensure the file name and class name match");
				return false;
			}

			if (typeDefinition.HasAttribute (nameof (NotATestCaseAttribute))) {
				return false;
			}

			// Verify the class as a static main method
			var mainMethod = typeDefinition.Methods.FirstOrDefault (m => m.Name.ToLower() == "main");

			if (mainMethod == null) {
				Console.WriteLine ($"{typeDefinition} in {sourceFile} is missing a Main() method");
				return false;
			}

			if (!mainMethod.IsStatic) {
				Console.WriteLine ($"The Main() method for {typeDefinition} in {sourceFile} should be static");
				return false;
			}

			testCase = potentialCase;
			return true;
		}

		private static TypeDefinition FindTypeDefinition (AssemblyDefinition caseAssemblyDefinition, TestCase testCase)
		{
			var typeDefinition = caseAssemblyDefinition.MainModule.GetType (testCase.ReconstructedFullTypeName);

			// For all of the Test Cases, the full type name we constructed from the directory structure will be correct and we can successfully find
			// the type from GetType.
			if (typeDefinition != null)
				return typeDefinition;

			// However, some of types are supporting types rather than test cases.  and may not follow the standardized naming scheme of the test cases
			// We still need to be able to locate these type defs so that we can parse some of the metadata on them.
			// One example, Unity run's into this with it's tests that require a type UnityEngine.MonoBehaviours to exist.  This tpe is defined in it's own
			// file and it cannot follow our standardized naming directory & namespace naming scheme since the namespace must be UnityEngine
			foreach (var type in caseAssemblyDefinition.MainModule.Types) {
				//  Let's assume we should never have to search for a test case that has no namespace.  If we don't find the type from GetType, then o well, that's not a test case.
				if (string.IsNullOrEmpty (type.Namespace))
					continue;

				if (type.Name == testCase.Name) {
					// This isn't foolproof, but let's do a little extra vetting to make sure the type we found corresponds to the source file we are
					// processing.
					if (!testCase.SourceFile.ReadAllText ().Contains ($"namespace {type.Namespace}"))
						continue;

					return type;
				}
			}

			return null;
		}
	}
}