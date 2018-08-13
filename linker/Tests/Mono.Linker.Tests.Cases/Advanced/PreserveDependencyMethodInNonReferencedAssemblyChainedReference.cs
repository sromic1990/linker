﻿using System.Runtime.CompilerServices;
using Mono.Linker.Tests.Cases.Advanced.Dependencies;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Advanced {
	[IgnoreTestCase ("Currently failing")]
	[SetupCompileBefore ("base.dll", new [] { "Dependencies/PreserveDependencyMethodInNonReferencedAssemblyBase.cs" })]
	[SetupCompileBefore ("base2.dll", new [] { "Dependencies/PreserveDependencyMethodInNonReferencedAssemblyBase2.cs" }, references: new [] { "base.dll" }, addAsReference: false)]
	[SetupCompileBefore ("reference.dll", new [] { "Dependencies/PreserveDependencyMethodInNonReferencedAssemblyChainedLibrary.cs" }, references: new [] { "base.dll" }, addAsReference: false)]
	[SetupCompileBefore ("library.dll", new [] { "Dependencies/PreserveDependencyMethodInNonReferencedAssemblyChainedReferenceLibrary.cs" }, references: new [] { "base.dll", "reference.dll" }, addAsReference: false)]
	[KeptAssembly ("base.dll")]
	[KeptAssembly ("base2.dll")]
	[KeptAssembly ("library.dll")]
	[KeptAssembly ("reference.dll")]
	[KeptMemberInAssembly ("base.dll", typeof (PreserveDependencyMethodInNonReferencedAssemblyBase), "Method()")]
	[KeptMemberInAssembly ("base2.dll", "Mono.Linker.Tests.Cases.Advanced.Dependencies.PreserveDependencyMethodInNonReferencedAssemblyBase2", "Method()")]
	[KeptMemberInAssembly ("library.dll", "Mono.Linker.Tests.Cases.Advanced.Dependencies.PreserveDependencyMethodInNonReferencedAssemblyChainedReferenceLibrary", "Method()")]
	[KeptMemberInAssembly ("reference.dll", "Mono.Linker.Tests.Cases.Advanced.Dependencies.PreserveDependencyMethodInNonReferencedAssemblyChainedLibrary", "Method()")]
	public class PreserveDependencyMethodInNonReferencedAssemblyChainedReference {
		public static void Main ()
		{
			var obj = new Foo ();
			var val = obj.Method ();
			Dependency ();
		}
		
		[Kept]
		[PreserveDependency (".ctor()", "Mono.Linker.Tests.Cases.Advanced.Dependencies.PreserveDependencyMethodInNonReferencedAssemblyChainedReferenceLibrary", "library")]
		static void Dependency ()
		{
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (PreserveDependencyMethodInNonReferencedAssemblyBase))]
		class Foo : PreserveDependencyMethodInNonReferencedAssemblyBase {
			[Kept]
			public override string Method ()
			{
				return "Foo";
			}
		}
	}
}