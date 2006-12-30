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

	[Serializable]
	public class AgentKey
	{
		private Org org;
		private string name;

		public AgentKey()
		{
		}

		public AgentKey(Org org, string name)
		{
			this.org = org;
			this.name = name;
		}

		[BelongsTo("OrgId")]
		public virtual Org Org
		{
			get { return org; }
			set { org = value; }
		}

		[KeyProperty]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override string ToString()
		{
			return String.Join(":", new string[] {org.Id, name});
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
			if (!(obj is AgentKey))
			{
				return false;
			}
			AgentKey rhs = (AgentKey) obj;
			return (Org.Id == rhs.Org.Id || (Org.Id != null && Org.Id.Equals(rhs.Org.Id))) &&
			       (Name == rhs.Name || (Name != null && Name.Equals(rhs.Name)));
		}

		public override int GetHashCode()
		{
			return (Org.Id.GetHashCode() ^ Name.GetHashCode());
		}
	}
}