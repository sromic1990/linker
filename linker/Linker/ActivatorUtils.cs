using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.Linker {
	public static class ActivatorUtils {
		public enum CreateInstanceOverloadVariation {
			Unknown,
			Generic,
			Type,
			TypeBool,
			StringString,
			Other
		}

		public enum CreateInstanceCtorUsage {
			Unknown,
			Default,
			Custom
		}

		public static bool TryParseUsage (MethodReference method, out CreateInstanceOverloadVariation variation, out CreateInstanceCtorUsage ctorUsage)
		{
			variation = CreateInstanceOverloadVariation.Unknown;
			ctorUsage = CreateInstanceCtorUsage.Unknown;

			if (!IsMemberOfActivator (method))
				return false;

			if (method.Name != "CreateInstance")
				return false;

			// CreateInstance<T>()
			if (!method.HasParameters) {
				variation = CreateInstanceOverloadVariation.Generic;
				ctorUsage = CreateInstanceCtorUsage.Default;
				return true;
			}

			var parameters = method.Parameters;
			var parameter1 = parameters [0];
			
			// CreateInstance with first param Type
			if (IsType (parameter1.ParameterType)) {
				variation = CreateInstanceOverloadVariation.Type;
				ctorUsage = CreateInstanceCtorUsage.Custom;

				// CreateInstance(Type)
				if (parameters.Count == 1)
					ctorUsage = CreateInstanceCtorUsage.Default;
				
				// CreateInstance(Type, Bool)
				if (parameters.Count == 2 && IsBool (parameters [1].ParameterType)) {
					variation = CreateInstanceOverloadVariation.TypeBool;
					ctorUsage = CreateInstanceCtorUsage.Default;
				}

				return true;
			}

			// We don't care about any of the other 1 parameter overloads
			if (parameters.Count == 1) {
				variation = CreateInstanceOverloadVariation.Other;
				ctorUsage = CreateInstanceCtorUsage.Unknown;
				return true;
			}

			var parameter2 = parameters [1];

			// CreateInstance with string string starting params
			if (IsString (parameter1.ParameterType) && IsString (parameter2.ParameterType)) {
				variation = CreateInstanceOverloadVariation.StringString;
				ctorUsage = parameters.Count == 2 ? CreateInstanceCtorUsage.Default : CreateInstanceCtorUsage.Custom;
				return true;
			}

			variation = CreateInstanceOverloadVariation.Other;
			ctorUsage = CreateInstanceCtorUsage.Unknown;
			return true;
		}

		public static TypeDefinition EvaluateCreationForTypeVariation (Instruction callInstruction, CreateInstanceOverloadVariation variation, CreateInstanceCtorUsage ctorUsage)
		{
			if (variation != CreateInstanceOverloadVariation.Type && variation != CreateInstanceOverloadVariation.TypeBool)
				throw new ArgumentException ($"`{variation}` is not a supported variation");
			
			if (ctorUsage == CreateInstanceCtorUsage.Unknown)
				throw new ArgumentException ($"`{ctorUsage}` is not a supported ctor usage");

			// Don't have the stack eval abilities to figure out the type when non-default ctor overloads are used
			if (ctorUsage == CreateInstanceCtorUsage.Custom)
				return null;

			//
			// Expected pattern is
			// IL_0000: ldtoken Mono.Linker.Tests.Cases.Reflection.Activator.TypeOverload.DetectedByCreationType/Foo
			// IL_0005: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
			// IL_000a: call object [mscorlib]System.Activator::CreateInstance(class [mscorlib]System.Type)
			//
			var previousInstruction = callInstruction.Previous;
			if (previousInstruction == null)
				return null;
			
			// If it's the overload with a bool then we need to shift back 1 additional instruction
			if (variation == CreateInstanceOverloadVariation.TypeBool) {
				previousInstruction = previousInstruction.Previous;
				if (previousInstruction == null)
					return null;
			}
			
			var previousPreviousInstruction = previousInstruction.Previous;
			if (previousPreviousInstruction == null)
				return null;

			if (previousInstruction.OpCode.Code != Code.Call || previousPreviousInstruction.OpCode.Code != Code.Ldtoken)
				return null;

			return (previousPreviousInstruction.Operand as TypeReference)?.Resolve ();
		}

		public static TypeDefinition EvaluateCreationForStringStringVariation (LinkContext context, Instruction callInstruction, CreateInstanceOverloadVariation variation, CreateInstanceCtorUsage ctorUsage)
		{
			if (variation != CreateInstanceOverloadVariation.StringString)
				throw new ArgumentException ($"`{variation}` is not a supported variation");
			
			// Don't have the stack eval abilities to figure out the type when non-default ctor overloads are used
			if (ctorUsage == CreateInstanceCtorUsage.Custom)
				return null;
			
			//
			// Expected pattern is
			// IL_0000: ldstr "test"
			// IL_0005: ldstr "Mono.Linker.Tests.Cases.Reflection.Activator.StringOverload.SameAssembly+Foo"
			// IL_000a: call class [mscorlib]System.Runtime.Remoting.ObjectHandle [mscorlib]System.Activator::CreateInstance(string, string)
			//
			
			var previousInstruction = callInstruction.Previous;
			if (previousInstruction == null)
				return null;
			
			var previousPreviousInstruction = previousInstruction.Previous;
			if (previousPreviousInstruction == null)
				return null;

			if (previousInstruction.OpCode.Code != Code.Ldstr || previousPreviousInstruction.OpCode.Code != Code.Ldstr)
				return null;

			var typeName = previousInstruction.Operand as string;
			var assemblyName = previousPreviousInstruction.Operand as string;

			if (string.IsNullOrEmpty (typeName) || string.IsNullOrEmpty (assemblyName))
				return null;

			var assemblyDefinition = context.GetAssemblies ().FirstOrDefault (asm => asm.Name.Name == assemblyName);
			return assemblyDefinition?.MainModule.GetType (typeName.ToCecilName ());
		}

		public static TypeDefinition EvaluateCastType (Instruction callInstruction, CreateInstanceOverloadVariation variation)
		{
			if (variation == CreateInstanceOverloadVariation.Generic || variation == CreateInstanceOverloadVariation.Unknown)
				throw new ArgumentException ($"This method should not be used for variation : `{variation}`");

			var nextInstruction = callInstruction.Next;
			if (nextInstruction == null)
				return null;

			if (nextInstruction.OpCode.Code == Code.Isinst || nextInstruction.OpCode.Code == Code.Castclass) {
				var instanceBeingCastedToType = nextInstruction.Operand as TypeReference;
				if (instanceBeingCastedToType == null)
					return null;

				return ResolveCastType (instanceBeingCastedToType);
			}

			return null;
		}

		public static MethodDefinition [] CollectConstructorsToMarkForActivatorCreateInstanceUsage (TypeDefinition type, CreateInstanceCtorUsage usage)
		{
			if (usage == CreateInstanceCtorUsage.Default)
				return type.Methods.Where (MethodDefinitionExtensions.IsDefaultConstructor).ToArray ();

			return type.Methods.Where (m => m.IsConstructor).ToArray ();
		}

		static TypeDefinition ResolveCastType (TypeReference activationCastType)
		{
			if (!activationCastType.IsGenericInstance && !activationCastType.IsGenericParameter)
				return activationCastType.Resolve ();

			if (activationCastType is GenericParameter genericParameter) {
				if (!genericParameter.HasConstraints)
					return null;

				return genericParameter.Constraints [0].Resolve ();
			}

			return null;
		}

		static bool IsMemberOfActivator (MemberReference member)
		{
			return member.DeclaringType.Name == "Activator" && member.DeclaringType.Namespace == "System";
		}
		
		static bool IsString (TypeReference type)
		{
			return type.Namespace == "System" && type.Name == "String";
		}
			
		static bool IsBool (TypeReference type)
		{
			return type.Namespace == "System" && type.Name == "Boolean";
		}

		static bool IsType (TypeReference type)
		{
			return type.Namespace == "System" && type.Name == "Type";
		}
	}
}