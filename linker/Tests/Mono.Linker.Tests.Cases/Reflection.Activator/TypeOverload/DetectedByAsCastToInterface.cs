using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload {
	[IgnoreTestCase("TODO by Mike: Not supported yet")]
	public class DetectedByAsCastToInterface {
		public static void Main ()
		{
			var tmp = System.Activator.CreateInstance (UndetectableWayOfGettingType ()) as IFoo;
			HereToUseCreatedInstance (tmp);
		}
		
		[Kept]
		static void HereToUseCreatedInstance (object arg)
		{
		}

		[Kept]
		static Type UndetectableWayOfGettingType ()
		{
			return typeof (Foo);
		}

		interface IFoo
		{
		}

		[Kept]
		[KeptMember (".ctor()")]
		class Foo : IFoo {
		}
	}
}