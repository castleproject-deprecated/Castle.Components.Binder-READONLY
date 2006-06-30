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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Text;
	using Castle.Model.Configuration;
	using System.Reflection;

	/// <summary>
	/// Perform the conversion by mapping the configuration values
	/// to the struct properties.
	/// </summary>
	[Serializable]
	public class ValueTypeConverter : AbstractTypeConverter
	{
		public override bool CanHandleType(Type type)
		{
			// a struct of some kind.
			return type.IsValueType && type.IsPrimitive == false;
		}

		public override object PerformConversion(string value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			object instance = Activator.CreateInstance(targetType);

			BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | 
				BindingFlags.Public | BindingFlags.NonPublic;
			foreach (IConfiguration itemConfig in configuration.Children)
			{
				PropertyInfo propInfo = targetType.GetProperty(itemConfig.Name, bindingFlags);
				
				if (propInfo == null)//in configuration and not in the object? this is an error.
					throw new InvalidOperationException(
						string.Format("Could not find property {0} on type {1}", itemConfig.Name, targetType));
				
				if(propInfo.CanWrite==false)
					throw new InvalidOperationException(
						string.Format("Could not set property {0} on type {1}. It has no setter", 
						itemConfig.Name, targetType));

				object value = Context.Composition.PerformConversion(itemConfig.Value, propInfo.PropertyType);

				propInfo.SetValue(instance, value, null);
			}

			return instance;
		}
	}
}
