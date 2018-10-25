using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.AbstractClasses.NoKeptCtor {
	public class LinkXmlPreservesMethodsWhenNoOverrides {
		public static void Main ()
		{
			StaticMethodOnlyUsed.StaticMethod ();
		}

		abstract class Base {
		}

		[Kept]
		class StaticMethodOnlyUsed : Base {
			[Kept]
			public void OtherMethod ()
			{
			}

			[Kept]
			public static void StaticMethod ()
			{
			}
		}
	}
}