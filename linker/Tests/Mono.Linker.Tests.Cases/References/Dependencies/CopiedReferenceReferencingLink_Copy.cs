namespace Mono.Linker.Tests.Cases.References.Dependencies {
	public class CopiedReferenceReferencingLink_Copy {
		public static void ToKeepReferenceAtCompileTime ()
		{
		}

		public class UnusedClass : CopiedReferenceReferencingLink_Link.TypeForBase {
			public CopiedReferenceReferencingLink_Link.TypeForField Field;
			
			public CopiedReferenceReferencingLink_Link.TypeForParameter Property { get; set; }
			
			public CopiedReferenceReferencingLink_Link.TypeForReturnValue Method ()
			{
				return null;
			}

			public void Method2 (CopiedReferenceReferencingLink_Link.TypeForParameter param)
			{
			}

			public T Method3<T>(T arg) where T : CopiedReferenceReferencingLink_Link.TypeForGeneric1
			{
				return null;
			}

			public void Method4<T>(T arg)
			{
			}

			public void Helper()
			{
				Method4<CopiedReferenceReferencingLink_Link.TypeForGeneric3>(null);
			}
		}

		public class UnusedClassBase<T>
		{
		}

		public class UnusedClass2 : UnusedClassBase<CopiedReferenceReferencingLink_Link.TypeForGeneric2>
		{
		}
	}
}