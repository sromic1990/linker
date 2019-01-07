using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload.Create {
	/// <summary>
	/// We could do better in this case if we checked the stack to see what the bool value is, but it's not worth the effort right now
	/// </summary>
	public class SimpleCreateBoolOverload {
		public static void Main ()
		{
			System.Activator.CreateInstance (typeof (PublicAndTrue), true);
			System.Activator.CreateInstance (typeof (PublicAndFalse), false);

			System.Activator.CreateInstance (typeof (PrivateAndTrue), true);
			System.Activator.CreateInstance (typeof (PrivateAndFalse), false);
		}

		[Kept]
		class PublicAndTrue {
			public PublicAndTrue ()
			{
			}

			public PublicAndTrue (int arg)
			{
			}
		}
		
		[Kept]
		class PublicAndFalse {
			public PublicAndFalse ()
			{
			}

			public PublicAndFalse (int arg)
			{
			}
		}
		
		[Kept]
		class PrivateAndTrue {
			private PrivateAndTrue ()
			{
			}

			public PrivateAndTrue (int arg)
			{
			}
		}
		
		[Kept]
		class PrivateAndFalse {
			private PrivateAndFalse ()
			{
			}

			public PrivateAndFalse (int arg)
			{
			}
		}
	}
}