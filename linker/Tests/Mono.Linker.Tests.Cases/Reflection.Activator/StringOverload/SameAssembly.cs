using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload {
	public class SameAssembly {
		public static void Main ()
		{
			var tmp = System.Activator.CreateInstance ("test", "Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload.SameAssembly+Foo");
			HereToUseCreatedInstance (tmp);
		}

		[Kept]
		static void HereToUseCreatedInstance (object arg)
		{
		}

		[Kept]
		class Foo {
			[Kept]
			public Foo ()
			{
			}

			public Foo (int arg)
			{
			}
		}
	}
}