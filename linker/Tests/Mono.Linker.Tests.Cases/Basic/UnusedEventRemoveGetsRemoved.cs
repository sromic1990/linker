using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic {
	class UnusedEventRemoveGetsRemoved {
		public static void Main ()
		{
			var tmp = new Foo ();
			tmp.Bar += Tmp_Bar;
		}

		[Kept]
		private static void Tmp_Bar (object sender, EventArgs e)
		{
		}

		[KeptMember (".ctor()")]
		public class Foo
		{
			[Kept]
			[KeptBackingField]
			[KeptEventAddMethod]
			public event EventHandler<EventArgs> Bar;
		}
	}
}
