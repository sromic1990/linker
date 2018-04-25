//
// LoadReferencesStep.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2007 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace Mono.Linker.Steps {

	public class LoadReferencesStep : BaseStep {
		
		readonly Dictionary<AssemblyNameDefinition, AssemblyDefinition> _references = new Dictionary<AssemblyNameDefinition, AssemblyDefinition> ();

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			ProcessReferences (assembly);
		}

		void ProcessReferences (AssemblyDefinition assembly)
		{
			if (_references.ContainsKey (assembly.Name))
				return;

			_references.Add (assembly.Name, assembly);

			foreach (AssemblyDefinition referenceDefinition in Context.ResolveReferences (assembly)) {
				try {
					ProcessReferences (referenceDefinition);
				} catch (Exception ex) {
					throw new LoadException (string.Format ("Error while processing references of '{0}'", assembly.FullName), ex);
				}
			}
			
			ProcessReferencesInCustomAttributes (assembly);
		}
		
		void ProcessReferencesInCustomAttributes (AssemblyDefinition assembly)
		{
			ProcessCustomAttributes (assembly);
			ProcessCustomAttributes (assembly.MainModule);

			foreach (var type in assembly.AllDefinedTypes ()) {
				ProcessCustomAttributes (type);
				foreach (var member in type.AllMembers ())
					ProcessCustomAttributes (member);
			}
		}

		void ProcessCustomAttributes (ICustomAttributeProvider provider)
		{
			if (!provider.HasCustomAttributes)
				return;
			
			foreach (var arg in provider.AllCustomAttributeArguments ())
				ProcessAssembliesOfTypeReferences (arg);
		}

		void ProcessAssembliesOfTypeReferences (CustomAttributeArgument argument)
		{
			var at = argument.Type;
			if (at.IsArray) {
				var et = at.GetElementType ();
				if (et.Namespace != "System" || et.Name != "Type")
					return;

				if (argument.Value == null)
					return;

				foreach (var cac in (CustomAttributeArgument[]) argument.Value)
					ProcessAssemblyOf ((TypeReference) cac.Value);
			} else if (at.Namespace == "System" && at.Name == "Type") {
				ProcessAssemblyOf ((TypeReference) argument.Value);
			}
		}

		void ProcessAssemblyOf (TypeReference type)
		{
			var td = type.Resolve ();
			if (td == null)
				return;

			Context.Resolve (td.Module.Assembly.MainModule);
			ProcessReferences (td.Module.Assembly);
		}
	}
}
