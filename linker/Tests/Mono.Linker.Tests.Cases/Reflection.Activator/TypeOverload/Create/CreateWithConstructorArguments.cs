using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload.Create {
	/// <summary>
	/// Without full stack evaluation it's not possible to find the ldtoken for the first parameter without potentially picking up false positive ldtoken instructions
	/// and even if we did find it, we still wouldn't know the ctor parameter array length to even try and make a known match
	/// </summary>
	public class CreateWithConstructorArguments {
		public static void Main ()
		{
			var tmp = System.Activator.CreateInstance (typeof (Foo), new object [0]);
			HereToUseCreatedInstance (tmp);
		}

		[Kept]
		static void HereToUseCreatedInstance (object arg)
		{
		}

		[Kept]
		class Foo {
			public Foo ()
			{
			}

			public Foo (int arg)
			{
			}
		}
	}
}