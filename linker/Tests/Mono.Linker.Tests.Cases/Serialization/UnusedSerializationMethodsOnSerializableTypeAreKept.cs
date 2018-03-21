using System;
using System.Runtime.Serialization;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Serialization {
	public class UnusedSerializationMethodsOnSerializableTypeAreKept {
		public static void Main ()
		{
			var t = new Foo ();
			t.OtherMethod ();
		}
		
		[Kept]
		[Serializable]
		class Foo {
			[Kept]
			public Foo()
			{
			}
			
			[Kept]
			public Foo(SerializationInfo info, StreamingContext context)
			{
			}

			[Kept]
			public void OtherMethod()
			{
			}

			[Kept]
			[KeptAttributeAttribute (typeof (OnDeserializedAttribute))]
			[OnDeserialized]
			public void UnusedOnDeserializedMethod (StreamingContext context)
			{
			}

			[Kept]
			[KeptAttributeAttribute (typeof (OnDeserializingAttribute))]
			[OnDeserializing]
			public void UnusedOnDeserializingMethod (StreamingContext context)
			{
			}

			[Kept]
			[KeptAttributeAttribute (typeof (OnSerializedAttribute))]
			[OnSerialized]
			public void UnusedOnSerializedMethod (StreamingContext context)
			{
			}

			[Kept]
			[KeptAttributeAttribute (typeof (OnSerializingAttribute))]
			[OnSerializing]
			public void UnusedOnSerializingMethod (StreamingContext context)
			{
			}
		}
	}
}