using System.Runtime.InteropServices;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.Interfaces.OnReferenceType.NoKeptCtor {
	public class InterfaceMethodOnUnusedComTypePreservedByXml {
		public static void Main ()
		{
		}

		[ComImport]
		[Guid ("D7BB1889-3AB7-4681-A115-60CA9158FECA")]
		interface IFoo {
			void Foo ();
		}

		[ComImport]
		[Guid ("D7BB1889-3AB7-4681-A115-60CA9158FECA")]
		interface IBar {
			void Bar ();
		}

		[Kept]
		[KeptAttributeAttribute (typeof (GuidAttribute))]
		[ComImport]
		[Guid ("D7BB1889-3AB7-4681-A115-60CA9158FECA")]
		class A : IBar, IFoo
		{
			public extern void Foo();

			[Kept]
			public extern void Bar();
		}
	}
}