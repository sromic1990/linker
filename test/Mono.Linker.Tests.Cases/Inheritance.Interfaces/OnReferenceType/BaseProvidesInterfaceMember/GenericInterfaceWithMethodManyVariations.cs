using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.Interfaces.OnReferenceType.BaseProvidesInterfaceMember {
	public class GenericInterfaceWithMethodManyVariations {
		public static void Main ()
		{
			var fb = new FooWithBase ();
			IFoo<object> fo = fb;
			fo.Method (null);
			
			IFoo<int> fi = fb;
			fi.Method (0);
		}

		[Kept]
		interface IFoo<T> {
			[Kept]
			void Method (T arg);
		}

		[Kept]
		[KeptMember (".ctor()")]
		class BaseFoo {
			[Kept]
			public void Method (object arg)
			{
			}

			[Kept]
			public void Method (int arg)
			{
			}

			public void Method (string arg)
			{
			}

			public void Method (Bar arg)
			{
			}
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (BaseFoo))]
		[KeptInterface (typeof (IFoo<object>))]
		[KeptInterface (typeof (IFoo<int>))]
		class FooWithBase : BaseFoo, IFoo<object>, IFoo<int>, IFoo<string>, IFoo<Bar> {
		}

		class Bar {
		}
	}
}