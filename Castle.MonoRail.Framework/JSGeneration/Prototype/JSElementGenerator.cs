// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.JSGeneration.Prototype
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	/// <summary>
	/// Implementation for the <see cref="IJSElementGenerator"/>
	/// </summary>
	public class JSElementGenerator : DynamicDispatchSupport, IJSElementGenerator
	{
		private static readonly IDictionary DispMethods;

		private readonly JSGenerator generator;

		#region Type Constructor

		/// <summary>
		/// Collects the public methods
		/// </summary>
		static JSElementGenerator()
		{
			DispMethods = new HybridDictionary(true);

			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			MethodInfo[] methods = typeof(IJSElementGenerator).GetMethods(flags);

			PopulateAvailableMethods(DispMethods, methods);
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="JSElementGenerator"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <param name="root">The root.</param>
		public JSElementGenerator(JSGenerator generator, String root)
		{
			this.generator = generator;

			JSGenerator.Record(generator, "$('" + root + "')");
		}

		#endregion

		/// <summary>
		/// Gets the parent generator.
		/// </summary>
		/// <value>The parent generator.</value>
		public IJSGenerator ParentGenerator
		{
			get { return generator; }
		}

		#region DynamicDispatchSupport overrides

		/// <summary>
		/// Gets the generator methods.
		/// </summary>
		/// <value>The generator methods.</value>
		protected override IDictionary GeneratorMethods
		{
			get { return DispMethods; }
		}

		#endregion

		#region Dispatchable operations

		/// <summary>
		/// Replaces the content of the element.
		/// </summary>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.el('elementid').ReplaceHtml("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public void ReplaceHtml(object renderOptions)
		{
			generator.Call("update", generator.Render(renderOptions));
		}

		/// <summary>
		/// Replaces the entire element's content -- and not only its innerHTML --
		/// by the content evaluated.
		/// </summary>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.el('messagediv').Replace("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public void Replace(object renderOptions)
		{
			generator.Call("replace", generator.Render(renderOptions));
		}

		#endregion
	}
}
