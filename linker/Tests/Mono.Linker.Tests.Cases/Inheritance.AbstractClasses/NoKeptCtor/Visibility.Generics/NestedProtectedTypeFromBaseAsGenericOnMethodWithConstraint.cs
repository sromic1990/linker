using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Inheritance.AbstractClasses.NoKeptCtor.Visibility.Generics {
	public class NestedProtectedTypeFromBaseAsGenericOnMethodWithConstraint {
		public static void Main ()
		{
			StaticMethodOnlyUsed.StaticMethod ();
		}

		abstract class Base {
			protected class NestedType {
				public void Foo ()
				{
				}
			}
		}

		[Kept]
		class StaticMethodOnlyUsed : Base {
			[Kept]
			public static void StaticMethod ()
			{
				Helper<DerivedFromNested> ();
			}

			[Kept]
			private static Container<T> Helper<T> () where T : class
			{
				return null;
			}
			
			[Kept]
			class Container<T> {
			}

			[Kept]
			class DerivedFromNested : NestedType {
			}
		}
	}
}