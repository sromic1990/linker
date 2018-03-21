using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Serialization.StripSerialization {
	[SetupLinkerArgument ("--strip-serialization", "true")]
	public class UnusedSerializationCtorsOnSerializableTypeAreRemoved {
		public static void Main ()
		{
			var t = typeof (Foo).ToString ();
			var tmp = new Bar ();
			tmp.OtherMethod ();
		}

		[Kept]
		[RemovedPseudoAttribute (0x00002000)]
		[Serializable]
		class Foo {
			public Foo ()
			{
			}

			public Foo (SerializationInfo info, StreamingContext context)
			{
			}
		}
		
		[Kept]
		[RemovedPseudoAttribute (0x00002000)]
		[Serializable]
		class Bar {
			[Kept]
			public Bar ()
			{
			}

			public Bar (SerializationInfo info, StreamingContext context)
			{
			}

			[Kept]
			public void OtherMethod ()
			{
			}
		}
	}
}