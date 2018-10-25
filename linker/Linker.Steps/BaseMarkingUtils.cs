using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Mono.Linker.Steps {
	public static class BaseMarkingUtils {
		public static bool ShouldMarkTypeHierarchyForType (LinkContext context, TypeReference type, TypeDefinition visibilityScope)
		{
			if (!NeedToCheckTypeHierarchy (context, visibilityScope))
				return false;

			if (IsTypeHierarchyRequiredForType (context, type, visibilityScope))
				return true;
			
			// TODO : Nested?  Distinct?
			foreach (var generic in AllGenericTypesFor (type)) {
				if (ShouldMarkTypeHierarchyForType (context, generic, visibilityScope))
					return true;
			}

			return false;
		}
		
		public static bool ShouldMarkTypeHierarchyForTypeDefinition (LinkContext context, TypeDefinition type, TypeDefinition visibilityScope)
		{
			if (!NeedToCheckTypeHierarchy (context, visibilityScope))
				return false;

			if (IsTypeHierarchyRequiredForType(context, type, visibilityScope))
				return true;

			return false;
		}

		public static bool ShouldMarkTypeHierarchyForMethod (LinkContext context, MethodReference method, TypeDefinition visibilityScope)
		{
			if (!NeedToCheckTypeHierarchy (context, visibilityScope))
				return false;

			if (IsTypeHierarchyRequiredForMethod (context, method, visibilityScope))
				return true;
			
			if (ShouldMarkTypeHierarchyForType (context, method.ReturnType, visibilityScope))
				return true;

			foreach (var parameter in method.Parameters) {
				if (ShouldMarkTypeHierarchyForType (context, parameter.ParameterType, visibilityScope))
					return true;
			}

			foreach (var generic in AllGenericTypesFor (method)) {
				if (ShouldMarkTypeHierarchyForType (context, generic, visibilityScope))
					return true;
			}

			return false;
		}
		
		public static bool ShouldMarkTypeHierarchyForField (LinkContext context, FieldReference field, TypeDefinition visibilityScope)
		{
			if (!NeedToCheckTypeHierarchy (context, visibilityScope))
				return false;

			if (IsTypeHierarchyRequiredForField (context, field, visibilityScope))
				return true;
			
			if (ShouldMarkTypeHierarchyForType (context, field.FieldType, visibilityScope))
				return true;

			return false;
		}

		static bool NeedToCheckTypeHierarchy (LinkContext context, TypeDefinition visibilityScope)
		{
			// We do not currently change the base type of value types
			if (visibilityScope.IsValueType)
				return false;

			var basesOfScope = context.Annotations.GetBaseHierarchy (visibilityScope);

			// No need to do this for types derived from object.  It already has the lowest base class
			if (basesOfScope == null || basesOfScope.Count == 0)
				return false;

			return true;
		}

		static bool IsTypeHierarchyRequiredForField (LinkContext context, FieldReference field, TypeDefinition visibilityScope)
		{
			var resolved = field.Resolve ();
			if (resolved == null) {
				// Play it safe if we fail to resolve
				return true;
			}

			var basesOfScope = context.Annotations.GetBaseHierarchy (visibilityScope);
			var fromBase = basesOfScope.FirstOrDefault (b => resolved.DeclaringType == b);
			if (fromBase != null) {
				if (!resolved.IsStatic)
					return true;

				if (resolved.IsPublic)
					return false;

				// protected
				if (resolved.IsFamily)
					return true;

				// It must be internal.  Trust that if the compiler allowed it we can continue to access
				if (!resolved.IsPrivate)
					return false;
				
				return false;
			}
			
			if (IsTypeHierarchyRequiredForTypeDefinition (context, resolved.DeclaringType, visibilityScope))
				return true;

			return false;
		}
		
		static bool IsTypeHierarchyRequiredForMethod (LinkContext context, MethodReference method, TypeDefinition visibilityScope)
		{
			var resolved = method.Resolve ();
			if (resolved == null) {
				// Play it safe if we fail to resolve
				return true;
			}

			var basesOfScope = context.Annotations.GetBaseHierarchy (visibilityScope);
			var fromBase = basesOfScope.FirstOrDefault (b => resolved.DeclaringType == b);
			if (fromBase != null) {
				if (!resolved.IsStatic)
					return true;

				if (resolved.IsPublic)
					return false;

				// protected
				if (resolved.IsFamily)
					return true;

				// It must be internal.  Trust that if the compiler allowed it we can continue to access
				if (!resolved.IsPrivate)
					return false;
				
				return false;
			}
			
			// If the method wasn't declared on a base type of the current body, then we need to check if any of the methods types parents are base types
			// of the body
			if (IsTypeHierarchyRequiredForTypeDefinition (context, resolved.DeclaringType, visibilityScope))
				return true;

			return false;
		}
		
		static bool IsTypeHierarchyRequiredForType (LinkContext context, TypeReference type, TypeDefinition visibilityScope)
		{
			// A generic parameter can't cause the base type to be required
			if (type is GenericParameter)
				return false;
			
			var resolved = type.Resolve ();
			if (resolved == null) {
				// Play it safe if we fail to resolve
				return true;
			}

			if (IsTypeHierarchyRequiredForTypeDefinition (context, resolved, visibilityScope))
				return true;

			return false;
		}

		static bool IsTypeHierarchyRequiredForTypeDefinition (LinkContext context, TypeDefinition memberType, TypeDefinition visibilityScope)
		{
			var basesOfVisibilityScope = context.Annotations.GetBaseHierarchy (visibilityScope);
			var current = memberType;
			var parentsOfMemberType = new List<TypeDefinition> ();
			TypeDefinition foundBase = null;
			while (current != null) {
				foundBase = basesOfVisibilityScope.FirstOrDefault (b => current == b);
				parentsOfMemberType.Add (current);
				if (foundBase != null) {
					break;
				}

				current = current.DeclaringType;
			}

			if (foundBase == null)
				return false;

			if (memberType.IsPublic)
				return false;

			if (memberType.IsNestedPublic) {
				return parentsOfMemberType.Any (p => p != foundBase && !p.IsNestedPublic);
			}

			return true;
		}

		public static IEnumerable<TypeReference> AllGenericTypesFor (TypeReference type)
		{
			if (type is IGenericInstance genericInstanceType) {
				foreach (var generic in AllGenericTypesFor (genericInstanceType)) {
					yield return generic;
				}
			}
		}
		
		public static IEnumerable<TypeReference> AllGenericTypesFor (MethodReference method)
		{
			if (method is IGenericInstance genericInstanceMethod) {
				foreach (var generic in AllGenericTypesFor (genericInstanceMethod))
					yield return generic;
			}

			foreach (var generic in AllGenericTypesFor (method.DeclaringType))
				yield return generic;
		}
		
		private static IEnumerable<TypeReference> AllGenericTypesFor (IGenericInstance instance)
		{
			// TODO : Nested?  Distinct?
			foreach (var generic in instance.GenericArguments) {
				if (generic is GenericParameter)
					continue;

				yield return generic;
			}
		}

		public static IEnumerable<TypeReference> ConstraintsFor (TypeReference type)
		{
			if (type is IGenericInstance genericInstanceType) {
//				foreach (var generic in ConstraintsFor (genericInstanceType)) {
//					yield return generic;
//				}
				
				throw new NotImplementedException();
			}
			
			foreach (var genericParameter in type.GenericParameters)
			{
				foreach (var constraint in genericParameter.Constraints)
					yield return constraint;
			}
		}

		public static IEnumerable<TypeReference> ConstraintsFor (MethodReference method)
		{
			foreach (var genericParameter in method.GenericParameters)
			{
				foreach (var constraint in genericParameter.Constraints)
					yield return constraint;
			}
			
//			if (method is IGenericInstance genericInstanceMethod) {
//				throw new NotImplementedException();
//			}
			
			foreach (var generic in ConstraintsFor (method.DeclaringType))
				yield return generic;
		}

		public static HashSet<TypeReference> AllTypesAssociatedWithMethod (MethodReference method)
		{
			HashSet<TypeReference> values = new HashSet<TypeReference>();
			values.Add (method.DeclaringType);
			values.Add (method.ReturnType);
			foreach (var param in method.Parameters)
				values.Add (param.ParameterType);
			
			foreach(var constraint in ConstraintsFor (method))
				values.Add (constraint);
			
			foreach(var generic in AllGenericTypesFor (method))
				values.Add (generic);

			return values;
		}

		private static IEnumerable<TypeReference> ConstraintsFor (IGenericInstance instance)
		{
			foreach (var generic in instance.GenericArguments) {
				if (generic is GenericParameter parameter) {
					foreach (var constraint in parameter.Constraints)
						yield return constraint;
				}
			}
		}
	}
}