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

namespace Castle.ActiveRecord.Tests.Model.CompositeWithClassModel
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord.Framework;
	using NHibernate;

	[ActiveRecord("Groups")]
	public class Group : ActiveRecordBase
	{
		private int id;
		private String name;
		private IList agents;
		private IList orgs;
		private bool onSaveCalled, onUpdateCalled, onDeleteCalled, onLoadCalled;

		public Group()
		{
			agents = new ArrayList();
			orgs = new ArrayList();
		}

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		[HasAndBelongsToMany(typeof(Agent),
			Table = "GroupAgents",
			CompositeKeyColumnRefs = new String[] {"OrgId", "Name"},
			ColumnKey = "GroupId",
			Lazy = true,
			Inverse=true,
			Cascade = ManyRelationCascadeEnum.SaveUpdate)]
		public IList Agents
		{
			get { return agents; }
			set { agents = value; }
		}

		[HasAndBelongsToMany(typeof(Org),
			Table = "GroupOrgs",
			ColumnRef = "OrgId",
			ColumnKey = "GroupId",
			Lazy = true,
			Inverse=true,
			Cascade = ManyRelationCascadeEnum.SaveUpdate)]
		public IList Orgs
		{
			get { return orgs; }
			set { orgs = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordMediator.DeleteAll(typeof(Group));
		}

		public static void DeleteAll(string where)
		{
			ActiveRecordMediator.DeleteAll(typeof(Group), where);
		}

		public static Group[] FindAll()
		{
			return (Group[]) ActiveRecordMediator.FindAll(typeof(Group));
		}

		public static Group Find(int id)
		{
			return (Group) ActiveRecordMediator.FindByPrimaryKey(typeof(Group), id);
		}

		public static int FetchCount()
		{
			return CountAll(typeof(Group));
		}

		public static int FetchCount(string filter, params object[] args)
		{
			return CountAll(typeof(Group), filter, args);
		}

		public static bool Exists()
		{
			return Exists(typeof(Group));
		}

		public static bool Exists(string filter, params object[] args)
		{
			return Exists(typeof(Group), filter, args);
		}

		/// <summary>
		/// Lifecycle method invoked during Save of the entity
		/// </summary>
		protected override void OnSave()
		{
			onSaveCalled = true;
		}

		/// <summary>
		/// Lifecycle method invoked during Update of the entity
		/// </summary>
		protected override void OnUpdate()
		{
			onUpdateCalled = true;
		}

		/// <summary>
		/// Lifecycle method invoked during Delete of the entity
		/// </summary>
		protected override void OnDelete()
		{
			onDeleteCalled = true;
		}

		/// <summary>
		/// Lifecycle method invoked during Load of the entity
		/// </summary>
		protected override void OnLoad(object id)
		{
			onLoadCalled = true;
		}

		public bool OnSaveCalled
		{
			get { return onSaveCalled; }
		}

		public bool OnUpdateCalled
		{
			get { return onUpdateCalled; }
		}

		public bool OnDeleteCalled
		{
			get { return onDeleteCalled; }
		}

		public bool OnLoadCalled
		{
			get { return onLoadCalled; }
		}

		public ISession CurrentSession
		{
			get { return (ISession) ActiveRecordMediator.Execute(typeof(Group), new NHibernateDelegate(GrabSession), this); }
		}

		private object GrabSession(ISession session, object instance)
		{
			return session;
		}

		public void CustomAction()
		{
			ActiveRecordMediator.Execute(typeof(Group), new NHibernateDelegate(MyCustomMethod), this);
		}

		private object MyCustomMethod(ISession session, object blogInstance)
		{
			session.Delete(blogInstance);
			session.Flush();

			return null;
		}

		internal static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator.GetSessionFactoryHolder(); }
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is Group))
			{
				return false;
			}
			Group rhs = (Group)obj;
			return (this.Id == rhs.Id);
		}

		public override int GetHashCode()
		{
			return (Id.GetHashCode());
		}
	}
}