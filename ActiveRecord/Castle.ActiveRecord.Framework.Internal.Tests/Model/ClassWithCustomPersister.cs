// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System;
	using System.Collections;
	using NHibernate;
	using NHibernate.Cache;
	using NHibernate.Engine;
	using NHibernate.Id;
	using NHibernate.Metadata;
	using NHibernate.Persister;
	using NHibernate.Persister.Entity;
	using NHibernate.Type;

	[ActiveRecord("ClassA", Persister=typeof(CustomPersister))]
	public class ClassWithCustomPersister : ActiveRecordBase
	{
		private int id;
		private String name1;
		private String name2;
		private String name3;
		private String text;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(Insert=false, Update=false)]
		public string Name1
		{
			get { return name1; }
			set { name1 = value; }
		}

		[Property]
		public string Name2
		{
			get { return name2; }
			set { name2 = value; }
		}

		[Property(Unique=true, NotNull=true)]
		public string Name3
		{
			get { return name3; }
			set { name3 = value; }
		}

		[Property(ColumnType="StringClob")]
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
	}

	public class CustomPersister : IEntityPersister
	{
		public void PostInstantiate()
		{
			throw new NotImplementedException();
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsUnsaved(object obj)
		{
			throw new NotImplementedException();
		}

		public void SetPropertyValues(object obj, object[] values)
		{
			throw new NotImplementedException();
		}

		public object[] GetPropertyValues(object obj)
		{
			throw new NotImplementedException();
		}

		public void SetPropertyValue(object obj, int i, object value)
		{
			throw new NotImplementedException();
		}

		public object GetPropertyValue(object obj, int i)
		{
			throw new NotImplementedException();
		}

		public object GetPropertyValue(object obj, string name)
		{
			throw new NotImplementedException();
		}

		public IType GetPropertyType(string propertyName)
		{
			throw new NotImplementedException();
		}

		public int[] FindDirty(object[] x, object[] y, object owner, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public int[] FindModified(object[] old, object[] current, object owner, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object GetIdentifier(object obj)
		{
			throw new NotImplementedException();
		}

		public void SetIdentifier(object obj, object id)
		{
			throw new NotImplementedException();
		}

		public object GetVersion(object obj)
		{
			throw new NotImplementedException();
		}

		public object Instantiate(object id)
		{
			throw new NotImplementedException();
		}

		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection, object[] oldFields,
		                   object oldVersion, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsInstrumented(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object[] GetDatabaseSnapshot(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsUnsavedVersion(object[] values)
		{
			throw new NotImplementedException();
		}

		public bool IsInstance(object entity)
		{
			throw new NotImplementedException();
		}

		public void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory,
		                                                   EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object IdentifierSpace
		{
			get { throw new NotImplementedException(); }
		}

		public string[] PropertySpaces
		{
			get { throw new NotImplementedException(); }
		}

		public Type MappedClass
		{
			get { throw new NotImplementedException(); }
		}

		public string ClassName
		{
			get { throw new NotImplementedException(); }
		}

		public string RootEntityName
		{
			get { throw new NotImplementedException(); }
		}

		public string EntityName
		{
			get { throw new NotImplementedException(); }
		}

		public bool ImplementsLifecycle
		{
			get { throw new NotImplementedException(); }
		}

		public bool ImplementsValidatable
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasProxy
		{
			get { throw new NotImplementedException(); }
		}

		public Type ConcreteProxyClass
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCollections
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCascades
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsIdentifierAssignedByInsert
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasIdentifierProperty
		{
			get { throw new NotImplementedException(); }
		}

		public bool CanExtractIdOutOfEntity
		{
			get { throw new NotImplementedException(); }
		}

		bool IEntityPersister.IsVersioned
		{
			get { throw new NotImplementedException(); }
		}

		public IVersionType VersionType
		{
			get { throw new NotImplementedException(); }
		}

		public int VersionProperty
		{
			get { throw new NotImplementedException(); }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { throw new NotImplementedException(); }
		}

		public IType[] PropertyTypes
		{
			get { throw new NotImplementedException(); }
		}

		public string[] PropertyNames
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyUpdateability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyCheckability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyNullability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyInsertability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyVersionability
		{
			get { throw new NotImplementedException(); }
		}

		public CascadeStyle[] PropertyCascadeStyles
		{
			get { throw new NotImplementedException(); }
		}

		public IType IdentifierType
		{
			get { throw new NotImplementedException(); }
		}

		public string IdentifierPropertyName
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsCacheInvalidationRequired
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsLazyPropertiesCacheable
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCache
		{
			get { throw new NotImplementedException(); }
		}

		public ICacheConcurrencyStrategy Cache
		{
			get { throw new NotImplementedException(); }
		}

		public IClassMetadata ClassMetadata
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsBatchLoadable
		{
			get { throw new NotImplementedException(); }
		}

		public string[] QuerySpaces
		{
			get { throw new NotImplementedException(); }
		}

		public ISessionFactoryImplementor Factory
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsVersionPropertyGenerated
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasInsertGeneratedProperties
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasUpdateGeneratedProperties
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSelectBeforeUpdateRequired
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsVersioned
		{
			get { throw new NotImplementedException(); }
		}

		public IComparer VersionComparator
		{
			get { throw new NotImplementedException(); }
		}
	}
}
