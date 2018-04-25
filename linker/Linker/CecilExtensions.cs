using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Mono.Linker {
	public static class CecilExtensions {
		public static IEnumerable<TypeDefinition> AllDefinedTypes (this AssemblyDefinition assemblyDefinition)
		{
			return assemblyDefinition.Modules.SelectMany (m => m.AllDefinedTypes ());
		}

		public static IEnumerable<TypeDefinition> AllDefinedTypes (this ModuleDefinition moduleDefinition)
		{
			foreach (var typeDefinition in moduleDefinition.Types) {
				yield return typeDefinition;

				foreach (var definition in typeDefinition.AllDefinedTypes ())
					yield return definition;
			}
		}

		public static IEnumerable<TypeDefinition> AllDefinedTypes (this TypeDefinition typeDefinition)
		{
			foreach (var nestedType in typeDefinition.NestedTypes) {
				yield return nestedType;

				foreach (var definition in nestedType.AllDefinedTypes ())
					yield return definition;
			}
		}

		public static IEnumerable<IMemberDefinition> AllMembers (this ModuleDefinition module)
		{
			foreach (var type in module.AllDefinedTypes ()) {
				yield return type;

				foreach (var member in type.AllMembers ())
					yield return member;
			}
		}

		public static IEnumerable<IMemberDefinition> AllMembers (this TypeDefinition type)
		{
			foreach (var field in type.Fields)
				yield return field;

			foreach (var prop in type.Properties)
				yield return prop;

			foreach (var method in type.Methods)
				yield return method;

			foreach (var @event in type.Events)
				yield return @event;
		}

		public static IEnumerable<CustomAttributeArgument> AllCustomAttributeArguments(this ICustomAttributeProvider provider)
		{
			foreach (var customAttribute in provider.CustomAttributes)
			{
				foreach (var attribute in provider.CustomAttributes)
				{
					foreach (var arg in attribute.ConstructorArguments)
						yield return arg;

					foreach (var arg in attribute.Fields)
						yield return arg.Argument;

					foreach (var arg in attribute.Properties)
						yield return arg.Argument;
				}
			}
		}
	}
}