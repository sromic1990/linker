using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Serialization {
	public class UnusedSerializationCtorsOnSerializableTypeAreKept {
		public static void Main ()
		{
			var t = typeof (Foo).ToString ();
		}

		[Kept]
		[Serializable]
		class Foo {
			[Kept]
			public Foo ()
			{
			}
			
			[Kept]
			public Foo (SerializationInfo info, StreamingContext context)
			{
			}
		}
	}
}