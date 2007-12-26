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

namespace Castle.MonoRail.Framework.Test
{
	using System.Collections;
	using Castle.Core;
	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// Mocks the <see cref="IEmailTemplateService"/> calling 
	/// <see cref="MockEngineContext.AddMailTemplateRendered"/> to register
	/// the calls made to these methods
	/// </summary>
	public class MockEmailTemplateService : IEmailTemplateService
	{
		private readonly MockEngineContext context;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockEmailTemplateService"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public MockEmailTemplateService(MockEngineContext context)
		{
			this.context = context;
		}

		public Message RenderMailMessage(string templateName, string layoutName, IDictionary parameters)
		{
			context.AddMailTemplateRendered(templateName, parameters);

			return new Message("from", "to", "subject", "body");
		}

		public Message RenderMailMessage(string templateName, string layoutName, object parameters)
		{
			context.AddMailTemplateRendered(templateName, new ReflectionBasedDictionaryAdapter(parameters));

			return new Message("from", "to", "subject", "body");
		}

		public Message RenderMailMessage(string templateName, IEngineContext engineContext, IController controller,
		                                 IControllerContext controllerContext, bool doNotApplyLayout)
		{
			context.AddMailTemplateRendered(templateName, controllerContext.PropertyBag);

			return new Message("from", "to", "subject", "body");
		}
	}
}
