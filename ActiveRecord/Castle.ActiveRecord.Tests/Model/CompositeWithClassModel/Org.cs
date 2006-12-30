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

	[ActiveRecord("Orgs")]
	public class Org : ActiveRecordBase
	{
		private string id;
		private String description;
		private DateTime created;
		private IList groups;
		private IList agents;
		private int version;

		public Org()
		{
			groups = new ArrayList();
			agents = new ArrayList();
			created = DateTime.Now;
			version = -1;
		}

		public Org(string id, String description) : this()
		{
			this.id = id;
			this.description = description;
		}

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasAndBelongsToMany(typeof(Group),
			Table = "GroupOrgs",
			ColumnRef = "GroupId",
			ColumnKey = "OrgId",
			Lazy = true,
			Inverse = true,
			Cascade = ManyRelationCascadeEnum.SaveUpdate)]
		public IList Groups
		{
			get { return groups; }
			set { groups = value; }
		}

		[Version(UnsavedValue="negative")]
		public int Version
		{
			get { return version; }
			set { version = value; }
		}

		[Property(ColumnType="StringClob")]
		public String Contents
		{
			get { return description; }
			set { description = value; }
		}

		[HasMany(typeof(Agent), "OrgId", "Agents",
			Inverse=true,
			Lazy=true)]
		public IList Agents
		{
			get { return agents; }
			set { agents = value; }
		}

		[Property]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		protected override bool BeforeSave(IDictionary state)
		{
			state["Created"] = DateTime.Now;
			return true;
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(Org));
		}

		public static Org[] FindAll()
		{
			return (Org[]) FindAll(typeof(Org));
		}

		public static Org Find(string id)
		{
			return (Org) FindByPrimaryKey(typeof(Org), id);
		}

		public static int FetchCount()
		{
			return CountAll(typeof(Org));
		}

		public static int FetchCount(string filter, params object[] args)
		{
			return CountAll(typeof(Org), filter, args);
		}

		public void SaveWithException()
		{
			Save();

			throw new ApplicationException("Fake Exception");
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
			if (!(obj is Org))
			{
				return false;
			}
			Org rhs = (Org)obj;
			return (this.Id == rhs.Id || (this.Id != null && this.Id.Equals(rhs.Id)));
		}

		public override int GetHashCode()
		{
			return (Id.GetHashCode());
		}
	}
}