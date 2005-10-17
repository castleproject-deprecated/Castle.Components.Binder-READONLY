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

namespace Castle.Services.Security
{
	using System;
	using System.Security;
	using System.Security.Principal;
	using System.Threading;


	public class CustomPermission : IPermission, ISecurityEncodable
	{
		private readonly string permissionName;

		public CustomPermission(String permissionName)
		{
			this.permissionName = permissionName;
		}

		#region IPermission

		public IPermission Copy()
		{
			throw new NotImplementedException();
		}

		public IPermission Intersect(IPermission target)
		{
			throw new NotImplementedException();
		}

		public IPermission Union(IPermission target)
		{
			throw new NotImplementedException();
		}

		public bool IsSubsetOf(IPermission target)
		{
			throw new NotImplementedException();
		}

		public void Demand()
		{
			IPrincipal principal = Thread.CurrentPrincipal;

			IExtendedPrincipal extendedPrincipal = principal as IExtendedPrincipal;

			if (extendedPrincipal == null)
			{
				throw new SecurityException("The current principal does not implement IExtendedPrincipal");
			}


		}

		#endregion

		#region ISecurityEncodable

		public SecurityElement ToXml()
		{
			throw new NotImplementedException();
		}

		public void FromXml(SecurityElement e)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
