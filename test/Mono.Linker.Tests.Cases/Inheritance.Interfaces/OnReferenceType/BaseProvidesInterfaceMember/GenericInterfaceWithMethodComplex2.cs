using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.Interfaces.OnReferenceType.BaseProvidesInterfaceMember {
	public class GenericInterfaceWithMethodComplex2 {
		public static void Main ()
		{
			IFoo<object> f = new FooWithBase ();
			f.Method (null, 0);
		}

		[Kept]
		interface IFoo<T> {
			[Kept]
			void Method (T arg, int arg2);
		}

		[Kept]
		[KeptMember (".ctor()")]
		class BaseFoo<T1> {
			[Kept]
			public void Method (object arg, T1 arg2)
			{
			}

			// Although this method accepts the same (object, int) as the generic version will use,
			// it is the generic version that is called so this method should be removed
			public void Method (object arg, int arg2)
			{
			}
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (BaseFoo<>), "T1")]
		class BaseFoo2<T1> : BaseFoo<T1> {
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (BaseFoo2<>), "T1")]
		class BaseFoo3<T1> : BaseFoo2<T1> {
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (BaseFoo3<>), "T1")]
		class BaseFoo4<T1> : BaseFoo3<T1> {
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (BaseFoo4<int>))]
		[KeptInterface (typeof (IFoo<object>))]
		class FooWithBase : BaseFoo4<int>, IFoo<object> {
		}
	}
}