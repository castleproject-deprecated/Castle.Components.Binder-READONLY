// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Reflection;


	public class ActiveRecordModelBuilder
	{
		private static readonly BindingFlags DefaultBindingFlags = BindingFlags.DeclaredOnly|BindingFlags.Public|BindingFlags.Instance;

		/// <summary>
		/// Just to avoid the possibiliy of two 
		/// models pointing to the same type
		/// </summary>
		private IDictionary type2Model = new Hashtable();

		public ActiveRecordModel Create(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			if (type2Model.Contains(type))
			{
				return type2Model[type] as ActiveRecordModel;
			}

			ActiveRecordModel model = new ActiveRecordModel(type);

			type2Model[type] = model;

			PopulateModel(model, type);

			return model;
		}

		private void PopulateModel(ActiveRecordModel model, Type type)
		{
			ProcessActiveRecordAttribute(type, model);

			ProcessJoinedBaseAttribute(type, model);

			ProcessPrimaryKey(type, model);

			ProcessVersion(type, model);

			ProcessTimestamp(type, model);

			ProcessProperties(type, model);

			ProcessRelations(type, model);
		}

		private void ProcessJoinedBaseAttribute(Type type, ActiveRecordModel model)
		{
			model.IsJoinedSubClassBase = type.IsDefined( typeof(JoinedBaseAttribute), false );
		}

		private void ProcessTimestamp(Type type, ActiveRecordModel model)
		{
		}

		private void ProcessVersion(Type type, ActiveRecordModel model)
		{
		}

		private void PopulateActiveRecordAttribute(ActiveRecordAttribute attribute, ActiveRecordModel model)
		{
			if (attribute.DiscriminatorColumn != null)
			{
				model.IsDiscriminatorBase = true;

				if (attribute.DiscriminatorValue == null)
				{
					throw new ActiveRecordException( 
						String.Format("You must specify a discriminator value for the type {0}", model.Type.FullName) );
				}

				// We only pay attention to the DiscriminatorType specified if it's 
				// different than the default String type
				if (attribute.DiscriminatorType != null && !"String".Equals(attribute.DiscriminatorType) )
				{
					model.DiscriminatorType = attribute.DiscriminatorType;
				}

				model.DiscriminatorValue = attribute.DiscriminatorValue;

			}
			else if (attribute.DiscriminatorType != null)
			{
				throw new ActiveRecordException( 
					String.Format("The usage of DiscriminatorType for {0} is meaningless", model.Type.FullName) );
			}
		}

		private void ProcessRelations(Type type, ActiveRecordModel model)
		{
		}

		private void ProcessProperties(Type type, ActiveRecordModel model)
		{
			PropertyInfo[] props = type.GetProperties( DefaultBindingFlags );

			foreach( PropertyInfo prop in props )
			{
				if (prop.IsDefined( typeof(PropertyAttribute), false ))
				{
					PropertyAttribute propAtt = prop.GetCustomAttributes( typeof(PropertyAttribute), false )[0] as PropertyAttribute;

					model.Properties.Add( new PropertyModel( prop, propAtt ) );
				}
			}
		}

		private void ProcessPrimaryKey(Type type, ActiveRecordModel model)
		{
			PropertyInfo[] props = type.GetProperties( DefaultBindingFlags );

			foreach( PropertyInfo prop in props )
			{
				if (prop.IsDefined( typeof(PrimaryKeyAttribute), false ))
				{
					PrimaryKeyAttribute pkAtt = prop.GetCustomAttributes( typeof(PrimaryKeyAttribute), false )[0] as PrimaryKeyAttribute;

					model.Keys.Add( new PrimaryKeyModel( prop, pkAtt ) );
				}
			}
		}

		private void ProcessActiveRecordAttribute(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes( typeof(ActiveRecordAttribute), false );
	
			if (attrs.Length == 0)
			{
				throw new ActiveRecordException( String.Format("Type {0} is not using the ActiveRecordAttribute, which is obligatory.", type.FullName) );
			}
	
			ActiveRecordAttribute arAttribute = attrs[0] as ActiveRecordAttribute;
	
			PopulateActiveRecordAttribute(arAttribute, model);
		}
	}
}
