using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;
using Mono.Linker.Tests.Cases.References.Dependencies;

namespace Mono.Linker.Tests.Cases.References {
	[SetupCompileBefore ("linked.dll", new [] {typeof (CopiedReferenceReferencingLink_Link)})]
	[SetupCompileBefore ("copy.dll", new [] {typeof (CopiedReferenceReferencingLink_Copy)}, new [] {"linked.dll"})]

	[SetupLinkerAction ("copy", "copy")]
	
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForBase))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForField))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForParameter))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForReturnValue))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForProperty))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForGeneric1))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForGeneric2))]
	[KeptTypeInAssembly ("linked.dll", typeof (CopiedReferenceReferencingLink_Link.TypeForGeneric3))]
	public class CopiedReferenceReferencingLink {
		public static void Main ()
		{
			CopiedReferenceReferencingLink_Copy.ToKeepReferenceAtCompileTime ();
		}
	}
}