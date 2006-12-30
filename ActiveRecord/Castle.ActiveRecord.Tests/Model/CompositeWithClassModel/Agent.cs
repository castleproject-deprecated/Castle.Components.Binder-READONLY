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

	[ActiveRecord("Agents")]
	public class Agent : ActiveRecordBase
	{
		private AgentKey agentKey;
		private IList groups;
		private int version;

		public Agent()
		{
			groups = new ArrayList();
			version = -1;
		}

		public Agent(AgentKey key) : this()
		{
			agentKey = key;
		}

		[CompositeKey]
		public AgentKey Key
		{
			get { return agentKey; }
			set { agentKey = value; }
		}

		public string OrgId
		{
			get { return agentKey.Org.Id; }
		}

		public String Name
		{
			get { return agentKey.Name; }
		}

		[Version(UnsavedValue="negative")]
		public int Version
		{
			get { return version; }
			set { version = value; }
		}

		[HasAndBelongsToMany(typeof(Group),
			Table = "GroupAgents",
			ColumnRef = "GroupId",
			CompositeKeyColumnKeys = new String[] {"OrgId", "Name"},
			Lazy = true,
			Cascade = ManyRelationCascadeEnum.SaveUpdate)]
		public IList Groups
		{
			get { return groups; }
			set { groups = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(Agent));
		}

		public static Agent[] FindAll()
		{
			return (Agent[]) FindAll(typeof(Agent));
		}

		public static Agent Find(object key)
		{
			return (Agent) FindByPrimaryKey(typeof(Agent), key);
		}

		/// <summary>
		/// We make it visible only to for test cases' assertions 
		/// </summary>
		public static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator.GetSessionFactoryHolder(); }
		}

		public void CustomAction()
		{
			Execute(new NHibernateDelegate(MyCustomMethod));
		}

		private object MyCustomMethod(ISession session, object userInstance)
		{
			session.Delete(userInstance);
			session.Flush();

			return null;
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
			if (!(obj is Agent))
			{
				return false;
			}
			Agent rhs = (Agent)obj;
			return (this.Key == rhs.Key || (this.Key != null && this.Key.Equals(rhs.Key)));
		}

		public override int GetHashCode()
		{
			return (Key.GetHashCode());
		}
	}
}