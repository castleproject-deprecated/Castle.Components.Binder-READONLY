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
	using System.Reflection;
	using System.Web;
	using Castle.Core;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Descriptors;
	using Services;

	/// <summary>
	/// Coordinates the creation of new <see cref="MonoRailHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class MonoRailHttpHandlerFactory : IHttpHandlerFactory
	{
		private readonly static string CurrentEngineContextKey = "currentmrengineinstance";
		private readonly static string CurrentControllerKey = "currentmrcontroller";
		private readonly static string CurrentControllerContextKey = "currentmrcontrollercontext";
		private readonly object locker = new object();
		private ExtensionManager extensionManager;

		private IMonoRailConfiguration configuration;
		private IMonoRailContainer mrContainer;
		private IUrlTokenizer urlTokenizer;
		private IEngineContextFactory engineContextFactory;
		private IServiceProviderLocator serviceProviderLocator;
		private IControllerFactory controllerFactory;
		private IControllerContextFactory controllerContextFactory;
		private IStaticResourceRegistry staticResourceRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactory"/> class.
		/// </summary>
		public MonoRailHttpHandlerFactory() : this(ServiceProviderLocator.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactory"/> class.
		/// </summary>
		/// <param name="serviceLocator">The service locator.</param>
		public MonoRailHttpHandlerFactory(IServiceProviderLocator serviceLocator)
		{
			serviceProviderLocator = serviceLocator;
		}

		/// <summary>
		/// Returns an instance of a class that implements 
		/// the <see cref="T:System.Web.IHttpHandler"></see> interface.
		/// </summary>
		/// <param name="context">An instance of the <see cref="T:System.Web.HttpContext"></see> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses.</param>
		/// <param name="url">The <see cref="P:System.Web.HttpRequest.RawUrl"></see> of the requested resource.</param>
		/// <param name="pathTranslated">The <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath"></see> to the requested resource.</param>
		/// <returns>
		/// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
		/// </returns>
		public virtual IHttpHandler GetHandler(HttpContext context, 
		                                       String requestType, 
		                                       String url, String pathTranslated)
		{
			// TODO: Replace by a readwriterlock or something more efficient
			lock(locker)
			{
				if (mrContainer == null)
				{
					if (configuration == null)
					{
						configuration = ObtainConfiguration(context.ApplicationInstance);
					}

					IServiceProviderEx userServiceProvider = serviceProviderLocator.LocateProvider();

					mrContainer = CreateDefaultMonoRailContainer(userServiceProvider);
				}
			}

			EnsureServices();

			HttpRequest req = context.Request;

			UrlInfo urlInfo = urlTokenizer.TokenizeUrl(req.FilePath, req.PathInfo, req.Url, req.IsLocal, req.ApplicationPath);

			if (urlInfo.Area == "MonoRail" && urlInfo.Controller == "Files")
			{
				return new ResourceFileHandler(urlInfo, new DefaultStaticResourceRegistry());
			}

			// TODO: Identify requests for files (js files) and serve them directly bypassing the flow

			IEngineContext engineContext = engineContextFactory.Create(mrContainer, urlInfo, context);
			engineContext.AddService(typeof(IEngineContext), engineContext);

			IController controller = null;

//			try
			{
				controller = controllerFactory.CreateController(urlInfo.Area, urlInfo.Controller);
			}
//			catch(ControllerNotFoundException)
//			{
//				// TODO: Set the status code to 404 and Process 404 view/controller if available
//
//				throw;
//			}

			ControllerMetaDescriptor controllerDesc = 
				mrContainer.ControllerDescriptorProvider.BuildDescriptor(controller);

			IControllerContext controllerContext = 
				controllerContextFactory.Create(urlInfo.Area, urlInfo.Controller, urlInfo.Action, controllerDesc);

			context.Items[CurrentEngineContextKey] = engineContext;
			context.Items[CurrentControllerKey] = controller;
			context.Items[CurrentControllerContextKey] = controllerContext;

			if (IgnoresSession(controllerDesc.ControllerDescriptor))
			{
				return new SessionlessMonoRailHttpHandler(engineContext, controller, controllerContext);
			}
			else
			{
				return new MonoRailHttpHandler(engineContext, controller, controllerContext);
			}
		}

		/// <summary>
		/// Enables a factory to reuse an existing handler instance.
		/// </summary>
		/// <param name="handler">The <see cref="T:System.Web.IHttpHandler"></see> object to reuse.</param>
		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public IMonoRailConfiguration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		}

		/// <summary>
		/// Gets or sets the container.
		/// </summary>
		/// <value>The container.</value>
		public IMonoRailContainer Container
		{
			get { return mrContainer; }
			set { mrContainer = value; }
		}

		/// <summary>
		/// Gets or sets the service provider locator.
		/// </summary>
		/// <value>The service provider locator.</value>
		public IServiceProviderLocator ProviderLocator
		{
			get { return serviceProviderLocator; }
			set { serviceProviderLocator = value; }
		}

		/// <summary>
		/// Gets or sets the URL tokenizer.
		/// </summary>
		/// <value>The URL tokenizer.</value>
		public IUrlTokenizer UrlTokenizer
		{
			get { return urlTokenizer; }
			set { urlTokenizer = value; }
		}

		/// <summary>
		/// Gets or sets the engine context factory.
		/// </summary>
		/// <value>The engine context factory.</value>
		public IEngineContextFactory EngineContextFactory
		{
			get { return engineContextFactory; }
			set { engineContextFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller factory.
		/// </summary>
		/// <value>The controller factory.</value>
		public IControllerFactory ControllerFactory
		{
			get { return controllerFactory; }
			set { controllerFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller context factory.
		/// </summary>
		/// <value>The controller context factory.</value>
		public IControllerContextFactory ControllerContextFactory
		{
			get { return controllerContextFactory; }
			set { controllerContextFactory = value; }
		}

		/// <summary>
		/// Checks whether we should ignore session for the specified controller.
		/// </summary>
		/// <param name="controllerDesc">The controller desc.</param>
		/// <returns></returns>
		protected virtual bool IgnoresSession(ControllerDescriptor controllerDesc)
		{
			return controllerDesc.Sessionless;
		}

		/// <summary>
		/// Creates the default service container.
		/// </summary>
		/// <param name="userServiceProvider">The user service provider.</param>
		/// <returns></returns>
		protected virtual IMonoRailContainer CreateDefaultMonoRailContainer(IServiceProviderEx userServiceProvider)
		{
			DefaultMonoRailContainer container = new DefaultMonoRailContainer();

			container.UseServicesFromParent();
			container.Configure(Configuration);
			container.InstallMissingServices();
			container.StartExtensionManager();

			return container;
		}

		#region Static accessors

		/// <summary>
		/// Gets the current engine context.
		/// </summary>
		/// <value>The current engine context.</value>
		public static IEngineContext CurrentEngineContext
		{
			get { return HttpContext.Current.Items[CurrentEngineContextKey] as IEngineContext; }
		}

		/// <summary>
		/// Gets the current controller.
		/// </summary>
		/// <value>The current controller.</value>
		public static IController CurrentController
		{
			get { return HttpContext.Current.Items[CurrentControllerKey] as IController; }
		}

		/// <summary>
		/// Gets the current controller context.
		/// </summary>
		/// <value>The current controller context.</value>
		public static IControllerContext CurrentControllerContext
		{
			get { return HttpContext.Current.Items[CurrentControllerContextKey] as IControllerContext; }
		}

		#endregion

		private IMonoRailConfiguration ObtainConfiguration(HttpApplication appInstance)
		{
			IMonoRailConfiguration config = MonoRailConfiguration.GetConfig();

			if (config == null)
			{
				MethodInfo mrConfigurer = appInstance.GetType().GetMethod("MonoRail_Configure");

				if (mrConfigurer != null)
				{
					config = new MonoRailConfiguration();

					if (mrConfigurer.IsStatic)
					{
						mrConfigurer.Invoke(null, new object[] { config });
					}
					else
					{
						mrConfigurer.Invoke(appInstance, new object[] { config });
					}
				}
			}

			if (config == null)
			{
				throw new ApplicationException("You have to provide a small configuration to use " +
											   "MonoRail. This can be done using the web.config or " +
											   "your global asax (your class that extends HttpApplication) " +
											   "through the method MonoRail_Configure(IMonoRailConfiguration config). " +
											   "Check the samples or the documentation");
			}

			return config;
		}

		private void EnsureServices()
		{
			if (urlTokenizer == null)
			{
				urlTokenizer = mrContainer.UrlTokenizer;
			}
			if (engineContextFactory == null)
			{
				engineContextFactory = mrContainer.EngineContextFactory;
			}
			if (controllerFactory == null)
			{
				controllerFactory = mrContainer.ControllerFactory;
			}
			if (controllerContextFactory == null)
			{
				controllerContextFactory = mrContainer.ControllerContextFactory;
			}
			if (staticResourceRegistry == null)
			{
				staticResourceRegistry = mrContainer.StaticResourceRegistry;
			}
		}
	}
}
