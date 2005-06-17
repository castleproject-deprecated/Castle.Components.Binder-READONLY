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


	public class ActiveRecordModel : IVisitable
	{
		private readonly Type type;

		private String discriminatorType;
		private String discriminatorValue;
		private bool isJoinedSubClassBase;
		private bool isDiscriminatorBase;
		private IList keys = new ArrayList();
		private IList properties = new ArrayList();

		public ActiveRecordModel(Type type)
		{
			this.type = type;
		}

		public Type Type
		{
			get { return type; }
		}

		public bool IsJoinedSubClassBase
		{
			get { return isJoinedSubClassBase; }
			set { isJoinedSubClassBase = value; }
		}

		public bool IsDiscriminatorBase
		{
			get { return isDiscriminatorBase; }
			set { isDiscriminatorBase = value; }
		}

		public String DiscriminatorType
		{
			get { return discriminatorType; }
			set { discriminatorType = value; }
		}

		public String DiscriminatorValue
		{
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		public IList Keys
		{
			get { return keys; }
		}

		public IList Properties
		{
			get { return properties; }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitModel(this);
		}

		#endregion
	}
}
