// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;
	using System.Text;

	public class PrototypeHelper : AbstractHelper
	{
		public class JSGenerator
		{
			private StringBuilder lines = new StringBuilder();
			private static IDictionary GeneratorMethods;

			static JSGenerator()
			{
				GeneratorMethods = new HybridDictionary(true);

				MethodInfo[] methods =
					typeof(JSGenerator).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				foreach(MethodInfo method in methods)
				{
					GeneratorMethods[method.Name] = String.Empty;
				}
			}

			public void ReplaceHtml(String id, IDictionary renderOptions)
			{
				Call("Element.update", id);
			}

			public override string ToString()
			{
				return lines.ToString();
			}

			private void Call(object function, params string[] args)
			{
				Record(function + "(" + BuildJSArguments(args) + ")");
			}

			private void Record(string line)
			{
				lines.AppendFormat("{0};\r\n", line);
			}

			private static string BuildJSArguments(string[] args)
			{
				if (args == null || args.Length == 0) return String.Empty;

				StringBuilder tempBuffer = new StringBuilder();

				bool comma = false;

				foreach(String arg in args)
				{
					if (comma) tempBuffer.Append(',');

					tempBuffer.Append(arg);

					if (!comma) comma = true;
				}

				return tempBuffer.ToString();
			}

			public static bool IsGeneratorMethod(string method)
			{
				return GeneratorMethods.Contains(method);
			}

			public static void Dispatch(JSGenerator generator, string method, params object[] args)
			{
				// TODO: Dispatch implementation
			}
		}
	}
}
