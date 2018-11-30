using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload {
	[SetupCompileBefore ("other.dll", new [] {"Dependencies/OtherAssembly.cs"}, addAsReference: false)]

	// In this case we won't mark anything because we can't cause a new assembly to be pulled in during the mark step
	[RemovedMemberInAssembly ("other.dll", "Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload.Dependencies.OtherAssembly/Foo", ".ctor()")]
	public class UnreferencedAssembly {
		public static void Main()
		{
			var tmp = System.Activator.CreateInstance ("test", "Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload.Dependencies.OtherAssembly+Foo");
		}
	}
}