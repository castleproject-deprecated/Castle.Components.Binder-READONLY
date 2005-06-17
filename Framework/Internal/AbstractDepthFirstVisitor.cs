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


	public abstract class AbstractDepthFirstVisitor : IVisitor
	{
		public void VisitNode(IVisitable visitable)
		{
			visitable.Accept(this);
		}

		public void VisitNodes(IEnumerable nodes)
		{
			foreach(IVisitable visitable in nodes)
			{
				VisitNode(visitable);
			}
		}

		#region IVisitor Members

		public virtual void VisitModel(ActiveRecordModel model)
		{
			VisitNodes( model.Keys );
			// Version
			// Timestamp
			VisitNodes( model.Properties );
		}

		public virtual void VisitPrimaryKey(PrimaryKeyModel model)
		{
		}

		public virtual void VisitProperty(PropertyModel model)
		{
		}

		#endregion
	}
}
