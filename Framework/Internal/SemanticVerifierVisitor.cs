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

	
	public class SemanticVerifierVisitor : AbstractDepthFirstVisitor
	{
		public override void VisitModel(ActiveRecordModel model)
		{
			if (model.IsDiscriminatorBase && model.IsJoinedSubClassBase)
			{
				throw new ActiveRecordException( String.Format(
					"Unfortunatelly you can't have a discriminator class " + 
					"and a joined subclass at the same time - check type {0}", model.Type.FullName) );
			}

// TODO:
//			if (pk.Generator == PrimaryKeyType.Foreign)
//			{
//				PropertyInfo hasOneProp = GetPropertyWithAttribute( prop.DeclaringType, typeof(HasOneAttribute) );
//
//				if (hasOneProp != null)
//				{
//					builder.AppendFormat("<param name=\"property\">{0}</param>\r\n", hasOneProp.Name);
//				}
//			}

			base.VisitModel(model);
		}
	}
}
