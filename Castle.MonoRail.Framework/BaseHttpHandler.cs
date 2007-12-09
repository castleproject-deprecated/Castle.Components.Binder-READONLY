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

namespace Castle.MonoRail.Framework
{
	using System.Web;

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class BaseHttpHandler : IHttpHandler
	{
		private readonly IController controller;
		private readonly IEngineContext engineContext;
		private readonly bool ignoreFlash;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHttpHandler"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="ignoreFlash">if set to <c>true</c> [ignore flash].</param>
		protected BaseHttpHandler(IController controller, IEngineContext engineContext, bool ignoreFlash)
		{
			this.controller = controller;
			this.engineContext = engineContext;
			this.ignoreFlash = ignoreFlash;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			Process(context);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}

		/// <summary>
		/// Performs the base work of MonoRail. Extracts
		/// the information from the URL, obtain the controller
		/// that matches this information and dispatch the execution
		/// to it.
		/// </summary>
		/// <param name="context">The context.</param>
		public virtual void Process(HttpContext context)
		{
			context.Response.Output.WriteLine("Hello");


//			ControllerLifecycleExecutor executor =
//				(ControllerLifecycleExecutor)context.Items[ControllerLifecycleExecutor.ExecutorEntry];
//
//			DefaultEngineContext contextImpl = (DefaultEngineContext)context;
//			contextImpl.ResolveRequestSession();
//
//			context.Items["mr.controller"] = executor.Controller;
//			context.Items["mr.flash"] = executor.Controller.Flash;
//			context.Items["mr.propertybag"] = executor.Controller.PropertyBag;
//			context.Items["mr.session"] = context.Session;
//
//			// At this point, the before filters were executed. 
//			// So we just need to perform the secondary initialization
//			// and invoke the action
//
//			try
//			{
//				if (executor.HasError) // Some error happened
//				{
//					executor.PerformErrorHandling();
//				}
//				else
//				{
//					executor.ProcessSelectedAction();
//				}
//			}
//			catch (Exception ex)
//			{
//				if (logger.IsErrorEnabled)
//				{
//					logger.Error("Error processing " + context.Url, ex);
//				}
//
//				throw;
//			}
//			finally
//			{
//				try
//				{
//					executor.Dispose();
//				}
//				finally
//				{
//					IControllerFactory controllerFactory =
//						(IControllerFactory)context.GetService(typeof(IControllerFactory));
//
//					controllerFactory.Release(executor.Controller);
//				}
//
//				if (logger.IsDebugEnabled)
//				{
//					Controller controller = executor.Controller;
//
//					logger.DebugFormat("Ending request process for '{0}'/'{1}.{2}' Extension '{3}' with url '{4}'",
//						controller.AreaName, controller.Name, controller.Action, context.UrlInfo.Extension, context.UrlInfo.UrlRaw);
//				}
//
//				// Remove items from flash before leaving the page
//				context.Flash.Sweep();
//
//				if (context.Flash.HasItemsToKeep)
//				{
//					context.Session[Flash.FlashKey] = context.Flash;
//				}
//				else if (context.Session.Contains(Flash.FlashKey))
//				{
//					context.Session.Remove(Flash.FlashKey);
//				}
//			}
		}

//		/// <summary>
//		/// Can be overriden so new semantics can be supported.
//		/// </summary>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		protected virtual UrlInfo ExtractUrlInfo(IHandlerContext context)
//		{
//			return context.UrlInfo;
//		}
//
//		/// <summary>
//		/// Gets the current context.
//		/// </summary>
//		/// <value>The current context.</value>
//		public static IHandlerContext CurrentContext
//		{
//			get
//			{
//				HttpContext context = HttpContext.Current;
//
//				// Are we in a web request?
//				if (context == null) return null;
//
//				return EngineContextModule.ObtainRailsEngineContext(context);
//			}
//		}
	}
}
