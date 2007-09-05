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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	
	public class ConfirmationValidator : AbstractValidator
	{
		private String _confirmationFieldOrProperty;

		public ConfirmationValidator(String confirmationFieldOrProperty)
		{
			_confirmationFieldOrProperty = confirmationFieldOrProperty;
		}

		public override bool Perform(object instance, object fieldValue)
		{
			object confValue = GetFieldOrPropertyValue(instance, _confirmationFieldOrProperty);

			if (confValue == null && (fieldValue == null || fieldValue.ToString().Length == 0))
			{
				return true;
			}
			else if (confValue == null)
			{
				return false;
			}

			if (fieldValue == null && (confValue == null || confValue.ToString().Length == 0))
			{
				return true;
			}
			else if (fieldValue == null)
			{
				return false;
			}

			return confValue.Equals(fieldValue);
		}

		protected override string BuildErrorMessage()
		{
			return String.Format("Field {0} doesn't match with confirmation.", Property.Name);
		}
	}
}
