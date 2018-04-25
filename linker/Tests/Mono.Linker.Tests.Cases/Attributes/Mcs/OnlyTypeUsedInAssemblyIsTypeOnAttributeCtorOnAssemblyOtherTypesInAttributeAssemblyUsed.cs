using Mono.Linker.Tests.Cases.Attributes.Dependencies;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

[assembly: KeptAttributeAttribute (typeof (AttributeDefinedInReference))]
[assembly: AttributeDefinedInReference (typeof (TypeDefinedInReference))]

namespace Mono.Linker.Tests.Cases.Attributes.Mcs {
	/// <summary>
	/// This exlicit mcs test exists because mcs does not add LibraryWithType as a reference to the test assembly.  This creates a special case
	/// that the linker needs to handle
	/// </summary>
	[SetupCSharpCompilerToUse ("mcs")]
	[SetupCompileBefore ("LibraryWithType.dll", new [] { typeof(TypeDefinedInReference) })]
	[SetupCompileBefore ("LibraryWithAttribute.dll", new [] { typeof (AttributeDefinedInReference) })]
	[KeptTypeInAssembly ("LibraryWithType.dll", typeof (TypeDefinedInReference))]
	[RemovedMemberInAssembly ("LibraryWithType.dll", typeof (TypeDefinedInReference), "Unused()")]
	[KeptMemberInAssembly ("LibraryWithAttribute.dll", typeof (AttributeDefinedInReference), ".ctor(System.Type)")]
	[KeptTypeInAssembly ("LibraryWithAttribute.dll", typeof (AttributeDefinedInReference_OtherType))]
	public class OnlyTypeUsedInAssemblyIsTypeOnAttributeCtorOnAssemblyOtherTypesInAttributeAssemblyUsed {
		public static void Main ()
		{
			// Use something in the attribute assembly so that the special behavior of not preserving a reference if the only thing that is marked
			// are attributes is not trigged
			AttributeDefinedInReference_OtherType.Method();
		}
	}
}