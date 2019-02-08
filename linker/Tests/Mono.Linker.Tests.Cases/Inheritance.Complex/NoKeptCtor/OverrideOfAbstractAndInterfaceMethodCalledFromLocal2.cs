using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.Complex.NoKeptCtor {
	public class OverrideOfAbstractAndInterfaceMethodCalledFromLocal2 {
		public static void Main ()
		{
			Foo b = HelperToMarkFooAndRequireBase ();
			b.Method ();
			IBar i = GetAnIBar ();
			i.Method ();
		}

		[Kept]
		static Foo HelperToMarkFooAndRequireBase ()
		{
			return null;
		}

		[Kept]
		static IBar GetAnIBar()
		{
			return null;
		}

		[Kept]
		abstract class Base {
			[Kept]
			public abstract void Method ();
		}

		[Kept]
		[KeptBaseType (typeof (Base))]
		class Foo : Base, IBar {
			[Kept]
			public override void Method ()
			{
				UsedByOverride ();
			}

			[Kept]
			void UsedByOverride ()
			{
			}
		}

		[Kept]
		interface IBar {
			[Kept]
			void Method();
		}
	}
}