using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Serialization.StripSerialization {
	[SetupLinkerArgument ("--strip-serialization", "true")]
	public class UnusedSerializationMethodsOnSerializableTypeAreRemoved {
		public static void Main ()
		{
			var t = new Foo ();
			t.OtherMethod ();
		}
		
		[Kept]
		[RemovedPseudoAttribute (0x00002000)]
		[Serializable]
		class Foo {
			[Kept]
			public Foo ()
			{
			}

			[Kept]
			public void OtherMethod ()
			{
			}

			[OnDeserialized]
			public void UnusedOnDeserializedMethod ()
			{
			}

			[OnDeserializing]
			public void UnusedOnDeserializingMethod ()
			{
			}

			[OnSerializing]
			public void UnusedOnSerializedMethod ()
			{
			}

			[OnSerializing]
			public void UnusedOnSerializingMethod ()
			{
			}
		}
	}
}