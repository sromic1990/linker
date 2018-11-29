using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.OtherOverloads {
	public class WithActivationContextDoesNothing {
		public static void Main ()
		{
			var tmp = System.Activator.CreateInstance (GetContext ());
			HereToUseCreatedInstance (tmp);
		}

		[Kept]
		static ActivationContext GetContext ()
		{
			return null;
		}
		
		[Kept]
		static void HereToUseCreatedInstance (object arg)
		{
		}
	}
}