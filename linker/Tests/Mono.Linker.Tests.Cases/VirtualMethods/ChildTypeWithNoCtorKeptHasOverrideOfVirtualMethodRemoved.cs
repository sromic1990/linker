using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.VirtualMethods {
	public class ChildTypeWithNoCtorKeptHasOverrideOfVirtualMethodRemoved {
		public static void Main ()
		{
			var p = new ParentClass ();
			var t = typeof (ChildClass).ToString ();
			p.Foo ();
		}

		[Kept]
		[KeptMember (".ctor()")]
		class ParentClass {
			[Kept]
			public virtual void Foo ()
			{
			}
		}

		[Kept]
		[KeptBaseType (typeof (ParentClass))]
		class ChildClass : ParentClass {
			public override void Foo ()
			{
			}
		}
	}
}