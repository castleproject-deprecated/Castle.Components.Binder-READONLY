// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System.IO;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultControllerContextFactory : IControllerContextFactory
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		/// <returns></returns>
		public IControllerContext Create(string area, string controller, string action,
		                                 ControllerMetaDescriptor metaDescriptor)
		{
			ControllerContext context = new ControllerContext(controller, area, action, metaDescriptor);

			context.ViewFolder = ResolveViewFolder(context, area, controller, action);
			context.SelectedViewName = ResolveDefaultViewSelection(context, area, controller, action);

			return context;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected virtual string ResolveViewFolder(ControllerContext context, string area, string controller, string action)
		{
			if (!string.IsNullOrEmpty(area))
			{
				return Path.Combine(area, controller);
			}

			return controller;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected virtual string ResolveDefaultViewSelection(ControllerContext context, string area, string controller,
		                                                     string action)
		{
			return Path.Combine(context.ViewFolder, action);
		}
	}
}