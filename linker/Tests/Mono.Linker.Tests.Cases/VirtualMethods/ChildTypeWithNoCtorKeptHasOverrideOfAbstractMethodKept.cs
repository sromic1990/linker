using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.VirtualMethods {
	public class ChildTypeWithNoCtorKeptHasOverrideOfAbstractMethodKept {
		public static void Main ()
		{
			Base p = new Child1 ();
			var t = typeof (Child2).ToString ();
			p.Foo ();
		}

		[Kept]
		[KeptMember(".ctor()")]
		abstract class Base {
			[Kept]
			public abstract void Foo ();
		}

		[Kept]
		[KeptMember (".ctor()")]
		[KeptBaseType (typeof (Base))]
		class Child1 : Base {
			[Kept]
			public override void Foo ()
			{
			}
		}

		[Kept]
		[KeptBaseType (typeof (Base))]
		class Child2 : Base {
			[Kept]
			public override void Foo ()
			{
			}
		}
	}
}