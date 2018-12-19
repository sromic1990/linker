using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;
using Mono.Linker.Tests.Cases.References.Dependencies;

namespace Mono.Linker.Tests.Cases.References {
	[SetupCompileBefore ("linked.dll", new [] { "Dependencies/CopiedReferenceReferencingLinkWithExportedType_Link.cs"}, addAsReference: false)]
	[SetupCompileBefore ("copy.dll",
		new [] {"Dependencies/CopiedReferenceReferencingLinkWithExportedType_Copy.cs"},
		new [] {"linked.dll"},
		new [] {"INCLUDE_EXPORT"})]

	[SetupLinkerAction ("copy", "copy")]

	[KeptTypeInAssembly ("linked.dll", "Mono.Linker.Tests.Cases.References.Dependencies.CopiedReferenceReferencingLinkWithExportedType_Link_Exported")]
	[RemovedTypeInAssembly ("linked.dll", "Mono.Linker.Tests.Cases.References.Dependencies.CopiedReferenceReferencingLinkWithExportedType_Link/Unused")]
	public class CopiedReferenceReferencingLinkWithExportedType {
		public static void Main ()
		{
			CopiedReferenceReferencingLinkWithExportedType_Copy.ToKeepReferenceAtCompileTime ();
		}
	}
}