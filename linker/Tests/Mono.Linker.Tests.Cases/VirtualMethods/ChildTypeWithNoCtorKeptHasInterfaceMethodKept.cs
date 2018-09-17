using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.VirtualMethods {
	public class ChildTypeWithNoCtorKeptHasInterfaceMethodKept {
		public static void Main ()
		{
			IBase p = new Child1 ();
			var t = typeof (Child2).ToString ();
			p.Foo ();
		}

		[Kept]
		interface IBase {
			[Kept]
			void Foo ();
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptInterface (typeof (IBase))]
		class Child1 : IBase {
			[Kept]
			public void Foo ()
			{
			}
		}

		[Kept]
		[KeptInterface (typeof (IBase))]
		class Child2 : IBase {
			[Kept]
			public void Foo ()
			{
			}
		}
	}
}