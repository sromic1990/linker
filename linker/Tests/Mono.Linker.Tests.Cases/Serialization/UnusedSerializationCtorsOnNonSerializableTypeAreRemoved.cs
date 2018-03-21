using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Serialization {
	public class UnusedSerializationCtorsOnNonSerializableTypeAreRemoved {
		public static void Main ()
		{
			var t = typeof (Foo).ToString ();
		}

		[Kept]
		class Foo {
			public Foo ()
			{
			}
			
			public Foo (SerializationInfo info, StreamingContext context)
			{
			}
		}
	}
}