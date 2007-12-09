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

namespace Castle.MonoRail.Framework.JSGeneration
{
	using System;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// DynamicDispatch support is an infrastructure 
	/// that mimics a dynamic language/environment. 
	/// It is not finished but the idea is to allow 
	/// plugins to add operations to the generators.
	/// </summary>
	public abstract class DynamicDispatchSupport
	{
		/// <summary>
		/// Populates the available methods.
		/// </summary>
		/// <param name="generatorMethods">The generator methods.</param>
		/// <param name="methods">The methods.</param>
		protected static void PopulateAvailableMethods(IDictionary generatorMethods, MethodInfo[] methods)
		{
			foreach(MethodInfo method in methods)
			{
				generatorMethods[method.Name] = method;
			}
		}

		/// <summary>
		/// Gets the generator methods.
		/// </summary>
		/// <value>The generator methods.</value>
		protected abstract IDictionary GeneratorMethods { get; }

		/// <summary>
		/// Determines whether [is generator method] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		/// 	<c>true</c> if [is generator method] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsGeneratorMethod(string method)
		{
			return GeneratorMethods.Contains(method);
		}

		/// <summary>
		/// Dispatches the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public object Dispatch(string method, params object[] args)
		{
			MethodInfo methodInfo = (MethodInfo) GeneratorMethods[method];

			ParameterInfo[] parameters = methodInfo.GetParameters();

			int paramArrayIndex = -1;

			for(int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo paramInfo = parameters[i];

				if (paramInfo.IsDefined(typeof(ParamArrayAttribute), true))
				{
					paramArrayIndex = i;
				}
			}

			try
			{
				return methodInfo.Invoke(this, BuildMethodArgs(methodInfo, args, paramArrayIndex));
			}
			catch(MonoRailException)
			{
				throw;
			}
			catch(Exception ex)
			{
				throw new MonoRailException("Error invoking method on generator. " +
				                            "Method invoked [" + method + "] with " + args.Length + " argument(s)", ex);
			}
		}

		private object[] BuildMethodArgs(MethodInfo method, object[] methodArguments, int paramArrayIndex)
		{
			ParameterInfo[] methodArgs = method.GetParameters();

			if (paramArrayIndex != -1)
			{
				Type arrayParamType = methodArgs[paramArrayIndex].ParameterType;

				object[] newParams = new object[methodArgs.Length];

				Array.Copy(methodArguments, newParams, methodArgs.Length - 1);

				if (methodArguments.Length < (paramArrayIndex + 1))
				{
					newParams[paramArrayIndex] = Array.CreateInstance(
						arrayParamType.GetElementType(), 0);
				}
				else
				{
					Array args = Array.CreateInstance(arrayParamType.GetElementType(), (methodArguments.Length + 1) - newParams.Length);

					Array.Copy(methodArguments, methodArgs.Length - 1, args, 0, args.Length);

					newParams[paramArrayIndex] = args;
				}

				methodArguments = newParams;
			}
			else
			{
				int expectedParameterCount = methodArgs.Length;

				if (methodArguments.Length < expectedParameterCount)
				{
					// Complete with nulls, assuming that parameters are optional

					object[] newArgs = new object[expectedParameterCount];

					Array.Copy(methodArguments, newArgs, methodArguments.Length);

					methodArguments = newArgs;
				}
			}

			return methodArguments;
		}
	}
}