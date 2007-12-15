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
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services.Utils;

	/// <summary>
	/// Default implementation of <see cref="IControllerLifecycleExecutor"/>
	/// <para>
	/// Handles the whole controller lifecycle in a request.
	/// </para>
	/// </summary>
	public class ControllerLifecycleExecutor : IControllerLifecycleExecutor, IServiceEnabledComponent
	{
		/// <summary>
		/// Key for the executor instance on <c>Context.Items</c>
		/// </summary>
		public const String ExecutorEntry = "mr.executor";

		private readonly Controller controller;
		private readonly IHandlerContext context;

		private ControllerMetaDescriptor metaDescriptor;
		private IServiceProvider serviceProvider;
		private ILogger logger = NullLogger.Instance;

		private IServiceProvider provider;

		/// <summary>
		/// The reference to the <see cref="IViewEngineManager"/> instance
		/// </summary>
		private IViewEngineManager viewEngineManager;

		private IResourceFactory resourceFactory;
		private IScaffoldingSupport scaffoldSupport;

		/// <summary>
		/// Reference to the <see cref="IFilterFactory"/> instance
		/// </summary>
		private IFilterFactory filterFactory;

		/// <summary>
		/// Reference to the <see cref="ITransformFilterFactory"/> instance
		/// </summary>
		private ITransformFilterFactory transformFilterFactory;

		/// <summary>
		/// Holds the filters associated with the action
		/// </summary>
		private FilterDescriptor[] filters;

		private IDynamicAction dynAction;
		private MethodInfo actionMethod;

		private bool skipFilters;
		private bool hasError;
		private bool hasConfiguredCache;
		private Exception exceptionToThrow;
		private IDictionary filtersToSkip;
		private ActionMetaDescriptor actionDesc;

		/// <summary>
		/// Initializes a new instance of 
		/// the <see cref="ControllerLifecycleExecutor"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public ControllerLifecycleExecutor(Controller controller, IHandlerContext context)
		{
			this.controller = controller;
			this.context = context;
		}

		#region IServiceEnabledComponent

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service provider</param>
		public void Service(IServiceProvider provider)
		{
			this.provider = provider;
			viewEngineManager = (IViewEngineManager) provider.GetService(typeof(IViewEngineManager));
			filterFactory = (IFilterFactory) provider.GetService(typeof(IFilterFactory));
			resourceFactory = (IResourceFactory) provider.GetService(typeof(IResourceFactory));
			scaffoldSupport = (IScaffoldingSupport) provider.GetService(typeof(IScaffoldingSupport));
			transformFilterFactory = (ITransformFilterFactory)provider.GetService(typeof(ITransformFilterFactory));
			
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(ControllerLifecycleExecutor));
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Disposes the filters and resources associated with a controller.
		/// </summary>
		public void Dispose()
		{
			DisposeFilters();

			ReleaseResources();
		}

		#endregion

		#region IControllerLifecycleExecutor
		
		/// <summary>
		/// Executes the method or the dynamic action with custom arguments
		/// </summary>
		/// <param name="actionArgs">The action args.</param>
		public void ProcessSelectedAction(IDictionary actionArgs)
		{
			bool actionSucceeded = false;

			try
			{
				bool canProceed = RunBeforeActionFilters();

				if (canProceed)
				{
					PrepareResources();
					PrepareTransformFilter();
					
					if (actionMethod != null)
					{
						controller.InvokeMethod(actionMethod, actionArgs);
					}
					else
					{
						dynAction.Execute(controller);
					}

					actionSucceeded = true;
					
					if (!hasConfiguredCache && 
					    !context.Response.WasRedirected &&
						actionDesc != null &&
						context.Response.StatusCode == 200)
					{
						// We need to prevent that a controller.Send
						// ends up configuring the cache for a different URL
						hasConfiguredCache = true;
						
						foreach(ICachePolicyConfigurer configurer in actionDesc.CacheConfigurers)
						{
							configurer.Configure(context.Response.CachePolicy);
						}
					}
				}
			}
			catch(ThreadAbortException)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("ThreadAbortException, process aborted");
				}

				hasError = true;

				return;
			}
			catch(Exception ex)
			{
				exceptionToThrow = ex;

				hasError = true;
			}

			RunAfterActionFilters();

			if (hasError && exceptionToThrow != null)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("Error processing action", exceptionToThrow);
				}

				if (context.Response.WasRedirected) return;

				PerformErrorHandling();
			}
			else if (actionSucceeded)
			{
				if (context.Response.WasRedirected) return;

				// If we haven't failed anywhere and no redirect was issued
				if (!hasError && !context.Response.WasRedirected)
				{
					// Render the actual view then cleanup
					ProcessView();
				}
			}
			
			RunAfterRenderFilters();
		}


		/// <summary>
		/// Performs the error handling:
		/// <para>
		/// - Tries to run the rescue page<br/>
		/// - Throws the exception<br/>
		/// </para>
		/// </summary>
		public void PerformErrorHandling()
		{
			if (context.Response.WasRedirected) return;
			
			// Try and perform the rescue
			if (PerformRescue(actionMethod, exceptionToThrow))
			{
				exceptionToThrow = null;
			}

			RaiseOnActionExceptionOnExtension();

			if (exceptionToThrow != null)
			{
				throw exceptionToThrow;
			}
		}

		/// <summary>
		/// Gets a value indicating whether an error has happened during controller processing
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if has error; otherwise, <see langword="false"/>.
		/// </value>
		public bool HasError
		{
			get { return hasError; }
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public Controller Controller
		{
			get { return controller; }
		}

		#endregion

		internal void InitializeControllerFieldsFromServiceProvider()
		{
			controller.InitializeFieldsFromServiceProvider(context);

			serviceProvider = context;

			metaDescriptor = controller.metaDescriptor;

			controller.viewEngineManager = viewEngineManager;			
		}

		#region Views and Layout

		/// <summary>
		/// Obtains the name of the default layout.
		/// </summary>
		/// <returns></returns>
		protected String ObtainDefaultLayoutName()
		{
			if (metaDescriptor.Layout != null)
			{
				return metaDescriptor.Layout.LayoutName;
			}
			else
			{
				String defaultLayout = String.Format("layouts/{0}", controller.Name);

				if (controller.HasTemplate(defaultLayout))
				{
					return controller.Name;
				}
			}

			return null;
		}

		private void ProcessView()
		{
			if (controller._selectedViewName != null)
			{
				viewEngineManager.Process(context, controller, controller._selectedViewName);
			}
		}

		#endregion

		#region Rescue

		/// <summary>
		/// Performs the rescue.
		/// </summary>
		/// <param name="method">The action (can be null in the case of dynamic actions).</param>
		/// <param name="ex">The exception.</param>
		/// <returns></returns>
		protected bool PerformRescue(MethodInfo method, Exception ex)
		{
			context.LastException = (ex is TargetInvocationException) ? ex.InnerException : ex;

			Type exceptionType = context.LastException.GetType();

			RescueDescriptor att = null;

			if (method != null)
			{
				ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);

				if (actionMeta.SkipRescue != null) return false;

				att = GetRescueFor(actionMeta.Rescues, exceptionType);
			}

			if (att == null)
			{
				att = GetRescueFor(metaDescriptor.Rescues, exceptionType);

				if (att == null) return false;
			}

			try
			{
				if (att.RescueController != null)
				{
					Controller rescueController = (Controller) Activator.CreateInstance(att.RescueController);

					using(ControllerLifecycleExecutor rescueExecutor = new ControllerLifecycleExecutor(rescueController, context))
					{
						rescueExecutor.Service(provider);
						ControllerDescriptor rescueDescriptor = ControllerInspectionUtil.Inspect(att.RescueController);
						rescueExecutor.InitializeController(rescueDescriptor.Area, rescueDescriptor.Name, att.RescueMethod.Name);
						rescueExecutor.SelectAction(att.RescueMethod.Name, rescueDescriptor.Name);

						IDictionary args = new Hashtable();
						IDictionary propertyBag = rescueController.PropertyBag;
						args["exception"] = propertyBag["exception"] = ex;
						args["controller"] = propertyBag["controller"] = controller;
						args["method"] = propertyBag["method"] = method;

						rescueExecutor.ProcessSelectedAction(args);
					}
				}
				else
				{
					controller._selectedViewName = Path.Combine("rescues", att.ViewName);
					ProcessView();
				}

				return true;
			}
			catch(Exception exception)
			{
				// In this situation, the rescue view could not be found
				// So we're back to the default error exibition

				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat("Failed to process rescue view. View name " +
					                   controller._selectedViewName, exception);
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the rescue for the specified exception type.
		/// </summary>
		/// <param name="rescues">The rescues.</param>
		/// <param name="exceptionType">Type of the exception.</param>
		/// <returns></returns>
		protected RescueDescriptor GetRescueFor(IList rescues, Type exceptionType)
		{
			if (rescues == null || rescues.Count == 0) return null;

			RescueDescriptor bestCandidate = null;

			foreach(RescueDescriptor rescue in rescues)
			{
				if (rescue.ExceptionType == exceptionType)
				{
					return rescue;
				}
				else if (rescue.ExceptionType != null &&
				         rescue.ExceptionType.IsAssignableFrom(exceptionType))
				{
					bestCandidate = rescue;
				}
			}

			return bestCandidate;
		}

		#endregion

		#region Extension

		/// <summary>
		/// Raises the on action exception on extension.
		/// </summary>
		protected void RaiseOnActionExceptionOnExtension()
		{
			ExtensionManager manager =
				(ExtensionManager) serviceProvider.GetService(typeof(ExtensionManager));

			manager.RaiseActionError(context);
		}

		#endregion

	}
}
