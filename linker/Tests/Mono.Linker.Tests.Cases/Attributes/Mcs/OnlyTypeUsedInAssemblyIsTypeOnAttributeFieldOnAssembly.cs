﻿using Mono.Linker.Tests.Cases.Attributes.Dependencies;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

[assembly: AttributeDefinedInReference (FieldType = typeof (TypeDefinedInReference))]

namespace Mono.Linker.Tests.Cases.Attributes.Mcs {
	/// <summary>
	/// In the case of attributes on assemblies, we expect both assemblies to be removed because we don't keep assembly level attributes
	/// when that is the only type marked in the assembly
	///
	/// This exlicit mcs test exists because mcs does not add LibraryWithType as a reference to the test assembly.  This creates a special case
	/// that the linker needs to handle
	/// </summary>
	[SetupCSharpCompilerToUse ("mcs")]
	[SetupCompileBefore ("LibraryWithType.dll", new [] { typeof(TypeDefinedInReference) })]
	[SetupCompileBefore ("LibraryWithAttribute.dll", new [] { typeof(AttributeDefinedInReference) })]
	[RemovedAssembly ("LibraryWithType.dll")]
	[RemovedAssembly ("LibraryWithAttribute.dll")]
	public class OnlyTypeUsedInAssemblyIsTypeOnAttributeFieldOnAssembly
	{
		public static void Main ()
		{
		}
	}
}