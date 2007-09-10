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

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.Core.Configuration;

	/// <summary>
	/// Usefull for test cases.
	/// </summary>
	public class InPlaceConfigurationSource : IConfigurationSource
	{
		private readonly IDictionary _type2Config = new Hashtable();
		private Type threadScopeInfoImplementation;
		private Type sessionFactoryHolderImplementation;
		private Type namingStrategyImplementation;
		private bool debug = false;
		private bool isLazyByDefault;
		private bool pluralizeTableNames;

		public InPlaceConfigurationSource()
		{
		}

		#region IConfigurationSource Members

		public Type ThreadScopeInfoImplementation
		{
			get { return threadScopeInfoImplementation; }
			set { threadScopeInfoImplementation = value; }
		}

		public Type SessionFactoryHolderImplementation
		{
			get { return sessionFactoryHolderImplementation; }
			set { sessionFactoryHolderImplementation = value; }
		}

		public Type NamingStrategyImplementation
		{
			get { return namingStrategyImplementation; }
			set { namingStrategyImplementation = value; }
		}

		public IConfiguration GetConfiguration(Type type)
		{
			return _type2Config[type] as IConfiguration;
		}

		public bool Debug
		{
			get { return debug; }
		}

		/// <summary>
		/// Gets a value indicating whether the entities should be lazy by default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if entities should be lazy by default; otherwise, <c>false</c>.
		/// </value>
		public bool IsLazyByDefault
		{
			get { return isLazyByDefault; }
		}

		/// <summary>
		/// Gets a value indicating whether table names are assumed plural by default. 
		/// </summary>
		/// <value>
		/// 	<c>true</c> if table names should be pluralized by default; otherwise, <c>false</c>.
		/// </value>
		public bool PluralizeTableNames
		{
			get { return pluralizeTableNames; }
			set { pluralizeTableNames = value; }
		}

		#endregion

		public void Add(Type type, IDictionary properties)
		{
			Add(type, ConvertToConfiguration(properties));
		}

		public void Add(Type type, IConfiguration config)
		{
			_type2Config[type] = config;
		}

		protected void SetUpThreadInfoType(bool isWeb, String customType)
		{
			Type threadInfoType = null;

			if (isWeb)
			{
				threadInfoType = typeof(WebThreadScopeInfo);
			}

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				threadInfoType = Type.GetType(typeName, false, false);

				if (threadInfoType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}

			ThreadScopeInfoImplementation = threadInfoType;
		}

		protected void SetUpSessionFactoryHolderType(String customType)
		{
			Type sessionFactoryHolderType = typeof(SessionFactoryHolder);

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				sessionFactoryHolderType = Type.GetType(typeName, false, false);

				if (sessionFactoryHolderType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}

			SessionFactoryHolderImplementation = sessionFactoryHolderType;
		}

		protected void SetUpNamingStrategyType(String customType)
		{
			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				Type namingStrategyType = Type.GetType(typeName, false, false);

				if (namingStrategyType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}

				NamingStrategyImplementation = namingStrategyType;
			}
		}

		protected void SetDebugFlag(bool isDebug)
		{
			debug = isDebug;
		}

		/// <summary>
		/// Set whatever entities are lazy by default or not.
		/// </summary>
		protected void SetIsLazyByDefault(bool lazyByDefault)
		{
			isLazyByDefault = lazyByDefault;
		}

		/// <summary>
		/// Sets the pluralizeTableNames flag.
		/// </summary>
		/// <param name="pluralize">if set to <c>true</c> Active Record will pluralize inferred table names.</param>
		protected void SetPluralizeTableNames(bool pluralize)
		{
			pluralizeTableNames = pluralize;
		}

		private IConfiguration ConvertToConfiguration(IDictionary properties)
		{
			MutableConfiguration conf = new MutableConfiguration("Config");

			foreach(DictionaryEntry entry in properties)
			{
				conf.Children.Add(new MutableConfiguration(entry.Key.ToString(), entry.Value.ToString()));
			}

			return conf;
		}
	}
}
