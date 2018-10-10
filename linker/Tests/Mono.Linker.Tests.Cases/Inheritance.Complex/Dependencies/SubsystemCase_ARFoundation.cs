using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Linker.Tests.Cases.Inheritance.Complex.Dependencies
{
	public class SubsystemCase_ARFoundation
	{
		private static List<XRFaceSubsystemDescriptor> s_FaceSubsystemDescriptors = new List<XRFaceSubsystemDescriptor>();
		
		public static void CreateSubsystems()
		{
			CreateFaceSubsystem();
		}

		public static XRFaceSubsystem CreateFaceSubsystem(string id = null)
		{
			return CreateStandaloneSubsystem<XRFaceSubsystemDescriptor, XRFaceSubsystem>(s_FaceSubsystemDescriptors, id);
		}
		
		private static TSubsystem CreateStandaloneSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id = null) where TDescriptor : SubsystemDescriptor<TSubsystem> where TSubsystem : Subsystem<TDescriptor>
		{
			if (descriptors == null)
			{
				throw new ArgumentNullException("descriptors");
			}
//			SubsystemManager.GetSubsystemDescriptors(descriptors);
			if (descriptors.Count > 0)
			{
				if (id == null)
				{
					TDescriptor descriptorToUse = descriptors[0];
					if (descriptors.Count > 1)
					{
						Type typeOfD = typeof(TDescriptor);
//						Debug.LogWarningFormat("Found {0} {1}s. Using \"{2}\"", descriptors.Count, typeOfD.Name, descriptorToUse.id);
					}
					return ((SubsystemDescriptor<TSubsystem>)descriptorToUse).Create();
				}
				foreach (TDescriptor descriptor in descriptors)
				{
					if (descriptor.id == id)
					{
						return ((SubsystemDescriptor<TSubsystem>)descriptor).Create();
					}
				}
			}
			return null;
		}
	}
}