using Mono.Cecil;

namespace Mono.Linker {
	public static class CustomAttributeExtensions {
		public static bool IsSerializationAttribute (this CustomAttribute ca)
		{
			var cat = ca.AttributeType;
			if (cat.Namespace != "System.Runtime.Serialization")
				return false;
			switch (cat.Name) {
				case "OnDeserializedAttribute":
				case "OnDeserializingAttribute":
				case "OnSerializedAttribute":
				case "OnSerializingAttribute":
					return true;
			}

			return false;
		}
	}
}