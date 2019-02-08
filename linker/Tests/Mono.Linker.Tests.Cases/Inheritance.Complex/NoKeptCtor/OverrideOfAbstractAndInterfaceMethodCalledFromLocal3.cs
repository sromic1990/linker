using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.Complex.NoKeptCtor {
	public class OverrideOfAbstractAndInterfaceMethodCalledFromLocal3 {
		public static void Main ()
		{
			Foo b = null;
			IBar i = b;
			i.Method ();
		}

		[Kept]
		abstract class Base {
			public abstract void Method ();
		}

		[Kept]
		[KeptBaseType (typeof (Base))]
		class Foo : Base, IBar {
			public override void Method ()
			{
				UsedByOverride ();
			}

			void UsedByOverride ()
			{
			}
		}

		[Kept]
		interface IBar {
			[Kept]
			void Method ();
		}
	}
}