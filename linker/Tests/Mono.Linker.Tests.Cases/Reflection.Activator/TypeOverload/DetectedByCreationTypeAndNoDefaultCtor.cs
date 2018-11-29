using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload {
	public class DetectedByCreationTypeAndNoDefaultCtor {
		public static void Main ()
		{
			var tmp = System.Activator.CreateInstance (typeof (Foo));
			HereToUseCreatedInstance (tmp);
		}

		[Kept]
		static void HereToUseCreatedInstance (object arg)
		{
		}

		[Kept]
		class Foo {
			public Foo (int arg)
			{
			}
		}
	}
}