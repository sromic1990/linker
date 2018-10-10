namespace Mono.Linker.Tests.Cases.Inheritance.Complex.Dependencies
{
	public class SubsystemCase_Face
	{
		
	}

	public abstract class XRFaceSubsystem : Subsystem<XRFaceSubsystemDescriptor>
	{
	}
	
	
	public class XRFaceSubsystemDescriptor : SubsystemDescriptor<XRFaceSubsystem>
	{
	}
}