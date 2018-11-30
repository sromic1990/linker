using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload.Create {
	/// <summary>
	/// monolinker does not have the stack evaluation capabilities to figure this out one because the array is on the stack before the ldtoken
	/// Without full stack evaluation it's not possible to find the ldtoken for the first parameter without potentially picking up false positive ldtokens
	/// </summary>
	public class DetectedByCreationTypeWithCtorArgumentsPreservesAllCtors {
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
			//[Kept] Requires better stack evaluation
			public Foo ()
			{
			}

			//[Kept] Requires better stack evaluation
			public Foo (int arg)
			{
			}
		}
	}
}