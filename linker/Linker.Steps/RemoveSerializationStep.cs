using System.Linq;
using Mono.Cecil;

namespace Mono.Linker.Steps {
	public class RemoveSerializationStep : BaseStep {
		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (Annotations.GetAction (assembly) == AssemblyAction.Link) {
				foreach (var type in assembly.MainModule.Types)
					ProcessType (type);
			}
		}
		
		static void ProcessType (TypeDefinition type)
		{
			if (type.IsSerializable)
			{
				type.IsSerializable = false;

				foreach (var method in type.Methods)
					RemoveCustomAttributesThatAreForSerialization (method);
			}

			foreach (var nested in type.NestedTypes)
				ProcessType (nested);
		}

		static void RemoveCustomAttributesThatAreForSerialization(MethodDefinition method)
		{
			if (!method.HasCustomAttributes)
				return;

			var attrsToRemove = method.CustomAttributes.Where (CustomAttributeExtensions.IsSerializationAttribute).ToArray ();
			foreach (var remove in attrsToRemove)
				method.CustomAttributes.Remove (remove);
		}
	}
}