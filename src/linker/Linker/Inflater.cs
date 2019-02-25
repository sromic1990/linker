using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Mono.Linker
{
	public class Inflater
	{
		public static TypeReference InflateType(GenericContext context, TypeReference typeReference)
		{
			var typeDefinition = InflateTypeWithoutException(context, typeReference);
			if (typeDefinition == null)
				throw new InvalidOperationException($"Unable to resolve a reference to the type '{typeReference.FullName}' in the assembly '{typeReference.Module.Assembly.FullName}'. Does this type exist in a different assembly in the project?");
	
			return typeDefinition;
		}
	
		public static GenericInstanceType InflateType(GenericContext context, TypeDefinition typeDefinition)
		{
			return ConstructGenericType(context, typeDefinition, typeDefinition.GenericParameters);
		}
	
		public static GenericInstanceType InflateType(GenericContext context, GenericInstanceType genericInstanceType)
		{
			var inflatedType = ConstructGenericType(context, genericInstanceType.Resolve(), genericInstanceType.GenericArguments);
			inflatedType.MetadataToken = genericInstanceType.MetadataToken;
			return inflatedType;
		}
	
		public static TypeReference InflateTypeWithoutException(GenericContext context, TypeReference typeReference)
		{
			var genericParameter = typeReference as GenericParameter;
			if (genericParameter != null)
			{
				if(genericParameter.Position >= context.Type.GenericArguments.Count)
					Console.WriteLine();
				
				if(context.Method == null && genericParameter.Type != GenericParameterType.Type)
					throw new ArgumentNullException();
				
				if (context.Method != null && genericParameter.Position >= context.Method.GenericArguments.Count)
					Console.WriteLine();
				
				var genericArgumentType = genericParameter.Type == GenericParameterType.Type
					? context.Type.GenericArguments[genericParameter.Position]
					: context.Method.GenericArguments[genericParameter.Position];
	
				var inflatedType = genericArgumentType;
				return inflatedType;
			}
			var genericInstanceType = typeReference as GenericInstanceType;
			if (genericInstanceType != null)
				return InflateType(context, genericInstanceType);
	
			var arrayType = typeReference as ArrayType;
			if (arrayType != null)
				return new ArrayType(InflateType(context, arrayType.ElementType), arrayType.Rank);
	
			var byReferenceType = typeReference as ByReferenceType;
			if (byReferenceType != null)
				return new ByReferenceType(InflateType(context, byReferenceType.ElementType));
	
			var pointerType = typeReference as PointerType;
			if (pointerType != null)
				return new PointerType(InflateType(context, pointerType.ElementType));
	
			var reqModType = typeReference as RequiredModifierType;
			if (reqModType != null)
				return InflateTypeWithoutException(context, reqModType.ElementType);
	
			var optModType = typeReference as OptionalModifierType;
			if (optModType != null)
				return InflateTypeWithoutException(context, optModType.ElementType);
	
			return typeReference.Resolve();
		}
	
		static GenericInstanceType ConstructGenericType(GenericContext context, TypeDefinition typeDefinition, IEnumerable<TypeReference> genericArguments)
		{
			var inflatedType = new GenericInstanceType(typeDefinition);
	
			foreach (var genericArgument in genericArguments)
				inflatedType.GenericArguments.Add(InflateType(context, genericArgument));
	
			return inflatedType;
		}

		public static MethodReference InflateMethod(GenericContext context, TypeDefinition candidateType, MethodDefinition methodDefinition)
		{
//			var declaringType = (TypeReference)methodDefinition.DeclaringType;
//			if (declaringType.Resolve().HasGenericParameters)
//				declaringType = InflateType(context, methodDefinition.DeclaringType);

			var declaringType = (TypeReference)candidateType;
			if (declaringType.Resolve().HasGenericParameters)
				declaringType = InflateType(context, candidateType);
	
			return ConstructGenericMethod(context, declaringType, methodDefinition, methodDefinition.GenericParameters);
		}
	
		public static MethodReference InflateMethod(GenericContext context, GenericInstanceMethod genericInstanceMethod)
		{
			var genericInstanceType = genericInstanceMethod.DeclaringType as GenericInstanceType;
			var inflatedType = genericInstanceType != null ? InflateType(context, genericInstanceType) : InflateType(context, genericInstanceMethod.DeclaringType);
	
			return ConstructGenericMethod(context, inflatedType, genericInstanceMethod.Resolve(), genericInstanceMethod.GenericArguments);
		}
	
		static MethodReference ConstructGenericMethod(GenericContext context, TypeReference declaringType, MethodDefinition method, IEnumerable<TypeReference> genericArguments)
		{
			var methodReference = new MethodReference(method.Name, method.ReturnType, declaringType) {HasThis = method.HasThis};
	
			foreach (var gp in method.GenericParameters)
				methodReference.GenericParameters.Add(new GenericParameter(gp.Name, methodReference));

			foreach (var p in method.Parameters)
			{
				if(method.FullName.Contains("Mono.Linker") && method.Name.Contains("Method"))
					Console.WriteLine();
				
				if (p.ParameterType.IsGenericParameter)
					methodReference.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, InflateType(context, p.ParameterType)));
				else
					methodReference.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, p.ParameterType));
			}
	
//			if (methodReference.Resolve() == null)
//				throw new Exception();

			if (genericArguments.Any())
			{

				var genericInstanceMethod = new GenericInstanceMethod(methodReference);
				foreach (var genericArgument in genericArguments)
					genericInstanceMethod.GenericArguments.Add(InflateType(context, genericArgument));

				return genericInstanceMethod;
			}

			return methodReference;
		}
		
		public class GenericContext
		{
			private readonly GenericInstanceType _type;
			private readonly GenericInstanceMethod _method;

			public GenericContext(GenericInstanceType type, GenericInstanceMethod method)
			{
				_type = type;
				_method = method;
			}

			public GenericInstanceType Type
			{
				get { return _type; }
			}

			public GenericInstanceMethod Method
			{
				get { return _method; }
			}
		}
	}
}