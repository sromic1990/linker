using System;

namespace Mono.Linker.Tests.Cases.Inheritance.Complex.Dependencies
{
	public class SubsystemCase_XRModule
	{
		
	}

	public interface ISubsystemDescriptor
	{
		
	}

	public interface ISubsystem
	{
		
	}
	
	public abstract class Subsystem : ISubsystem
	{
		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
	
	public abstract class Subsystem<TSubsystemDescriptor> : Subsystem where TSubsystemDescriptor : ISubsystemDescriptor
	{
	}

	public abstract class SubsystemDescriptor : ISubsystemDescriptor
	{
		public Type subsystemImplementationType { get; set; }
		public string id { get; set; }
	}

	public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptor where TSubsystem : Subsystem
	{
		public TSubsystem Create()
		{
			TSubsystem val = Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem;
			val.m_subsystemDescriptor = this;
			return val;
		}
	}
}