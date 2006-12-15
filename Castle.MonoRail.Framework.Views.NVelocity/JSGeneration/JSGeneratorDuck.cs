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

using NVelocity;

namespace Castle.MonoRail.Framework.Views.NVelocity.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.Helpers;

	public class JSGeneratorDuck : IDuck
	{
		private readonly PrototypeHelper.JSGenerator generator;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSGeneratorDuck"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public JSGeneratorDuck(PrototypeHelper.JSGenerator generator)
		{
			this.generator = generator;
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			return new JSElementGeneratorDuck(generator, propName);
		}

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="value">The value to assign.</param>
		public void SetInvoke(string propName, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		public object Invoke(string method, params object[] args)
		{
			if (PrototypeHelper.JSGenerator.IsGeneratorMethod(method))
			{
				PrototypeHelper.JSGenerator.Dispatch(generator, method, args);
			}
			else
			{
				throw new RailsException("Invalid method invoked on JSGenerator: " + method);
			}

			return null;
		}

		#endregion
	}

	public class JSElementGeneratorDuck : IDuck
	{
		public JSElementGeneratorDuck(PrototypeHelper.JSGenerator generator, string root)
		{
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			// Chain call ?

			return null;
		}

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="value">The value to assign.</param>
		public void SetInvoke(string propName, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		public object Invoke(string method, params object[] args)
		{
			if (method == "set") // $page.element
			{
				// assign
			}
			else
			{
				// chain?
			}

			return null;
		}

		#endregion
	}
}
