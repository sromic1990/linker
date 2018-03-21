using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Serialization.StripSerialization {
	[SetupLinkerArgument ("--strip-serialization", "true")]
	[SetupLinkerArgument ("--used-attrs-only", "true")]
	public class UsedSerializationMethodsOnTypeHaveAttributesRemovedWhenOnlyKeepUsedAttrs {
		public static void Main ()
		{
			var t = new Foo ();
			var context = new StreamingContext ();
			t.UnusedOnDeserializedMethod (context);
			t.UnusedOnDeserializingMethod (context);
			t.UnusedOnSerializedMethod (context);
			t.UnusedOnSerializingMethod (context);
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
			[OnDeserialized]
			public void UnusedOnDeserializedMethod (StreamingContext context)
			{
			}

			[Kept]
			[OnDeserializing]
			public void UnusedOnDeserializingMethod (StreamingContext context)
			{
			}

			[Kept]
			[OnSerialized]
			public void UnusedOnSerializedMethod (StreamingContext context)
			{
			}

			[Kept]
			[OnSerializing]
			public void UnusedOnSerializingMethod (StreamingContext context)
			{
			}
		}
	}
}