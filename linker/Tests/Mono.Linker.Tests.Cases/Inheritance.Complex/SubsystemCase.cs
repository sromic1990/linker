using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;
using Mono.Linker.Tests.Cases.Inheritance.Complex.Dependencies;

namespace Mono.Linker.Tests.Cases.Inheritance.Complex
{
	[SetupCompileBefore("xrmodule.dll", new [] {"Dependencies/SubsystemCase_XRModule.cs"})]
	[SetupCompileBefore("face.dll", new [] {"Dependencies/SubsystemCase_Face.cs"}, new [] {"xrmodule.dll"})]
	[SetupCompileBefore("foundation.dll", new [] {"Dependencies/SubsystemCase_ARFoundation.cs"}, new [] {"xrmodule.dll", "face.dll"})]
	[KeptMemberInAssembly("face.dll", typeof(XRFaceSubsystem), ".ctor()")]
	public class SubsystemCase
	{
		public static void Main()
		{
			SubsystemCase_ARFoundation.CreateSubsystems();
		}
	}
}