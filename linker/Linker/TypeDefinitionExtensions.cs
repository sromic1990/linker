using System.Linq;
using Mono.Cecil;

namespace Mono.Linker {
	public static class TypeDefinitionExtensions {
		public static MethodDefinition GetDefaultConstructor (this TypeDefinition type)
		{
			if (type == null || !type.HasMethods)
				return null;
			
			return type.Methods.FirstOrDefault (MethodDefinitionExtensions.IsDefaultConstructor);
		}
	}
}