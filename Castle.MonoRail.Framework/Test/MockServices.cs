namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections.Generic;
	using Castle.Components.Validator;
	using Providers;
	using Services;

	/// <summary>
	/// Pendent
	/// </summary>
	public class MockServices : IMonoRailServices
	{
		private IUrlTokenizer urlTokenizer;
		private IUrlBuilder urlBuilder;
		private ICacheProvider cacheProvider;
		private IEngineContextFactory engineContextFactory;
		private IControllerFactory controllerFactory;
		private IControllerContextFactory controllerContextFactory;
		private IControllerTree controllerTree;
		private IViewSourceLoader viewSourceLoader;
		private IFilterFactory filterFactory;
		private IControllerDescriptorProvider controllerDescriptorProvider;
		private IViewEngineManager viewEngineManager;
		private IValidatorRegistry validatorRegistry;
		private IActionSelector actionSelector;
		private IScaffoldingSupport scaffoldSupport;
		private IJSONSerializer jsonSerializer;
		private IStaticResourceRegistry staticResourceRegistry;
		private readonly Dictionary<Type, object> service2Impl = new Dictionary<Type, object>();

		/// <summary>
		/// Initializes a new instance of the <see cref="MockServices"/> class with default mock services.
		/// </summary>
		public MockServices() : this(new DefaultUrlBuilder(),
		                             new DefaultFilterFactory(),
		                             new ViewEngineManagerStub(),
		                             new DefaultActionSelector())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MockServices"/> class.
		/// </summary>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="filterFactory">The filter factory.</param>
		/// <param name="viewEngineManager">The view engine manager.</param>
		/// <param name="actionSelector">The action selector.</param>
		public MockServices(IUrlBuilder urlBuilder, IFilterFactory filterFactory, IViewEngineManager viewEngineManager,
		                    IActionSelector actionSelector)
		{
			this.urlBuilder = urlBuilder;
			this.filterFactory = filterFactory;
			this.viewEngineManager = viewEngineManager;
			this.actionSelector = actionSelector;

			controllerTree = new DefaultControllerTree();
			controllerFactory = new DefaultControllerFactory(controllerTree);
			staticResourceRegistry = new DefaultStaticResourceRegistry();

			controllerContextFactory = new DefaultControllerContextFactory();

			controllerDescriptorProvider = new DefaultControllerDescriptorProvider(
				new DefaultHelperDescriptorProvider(),
				new DefaultFilterDescriptorProvider(),
				new DefaultLayoutDescriptorProvider(),
				new DefaultRescueDescriptorProvider(),
				new DefaultResourceDescriptorProvider(),
				new DefaultTransformFilterDescriptorProvider());
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
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		/// <summary>
		/// Gets or sets the cache provider.
		/// </summary>
		/// <value>The cache provider.</value>
		public ICacheProvider CacheProvider
		{
			get { return cacheProvider; }
			set { cacheProvider = value; }
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
		/// Gets or sets the controller tree.
		/// </summary>
		/// <value>The controller tree.</value>
		public IControllerTree ControllerTree
		{
			get { return controllerTree; }
			set { controllerTree = value; }
		}

		/// <summary>
		/// Gets or sets the view source loader.
		/// </summary>
		/// <value>The view source loader.</value>
		public IViewSourceLoader ViewSourceLoader
		{
			get { return viewSourceLoader; }
			set { viewSourceLoader = value; }
		}

		/// <summary>
		/// Gets or sets the filter factory.
		/// </summary>
		/// <value>The filter factory.</value>
		public IFilterFactory FilterFactory
		{
			get { return filterFactory; }
			set { filterFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller descriptor provider.
		/// </summary>
		/// <value>The controller descriptor provider.</value>
		public IControllerDescriptorProvider ControllerDescriptorProvider
		{
			get { return controllerDescriptorProvider; }
			set { controllerDescriptorProvider = value; }
		}

		/// <summary>
		/// Gets or sets the view engine manager.
		/// </summary>
		/// <value>The view engine manager.</value>
		public IViewEngineManager ViewEngineManager
		{
			get { return viewEngineManager; }
			set { viewEngineManager = value; }
		}

		/// <summary>
		/// Gets or sets the validator registry.
		/// </summary>
		/// <value>The validator registry.</value>
		public IValidatorRegistry ValidatorRegistry
		{
			get { return validatorRegistry; }
			set { validatorRegistry = value; }
		}

		/// <summary>
		/// Gets or sets the action selector.
		/// </summary>
		/// <value>The action selector.</value>
		public IActionSelector ActionSelector
		{
			get { return actionSelector; }
			set { actionSelector = value; }
		}

		/// <summary>
		/// Gets or sets the scaffold support.
		/// </summary>
		/// <value>The scaffold support.</value>
		public IScaffoldingSupport ScaffoldSupport
		{
			get { return scaffoldSupport; }
			set { scaffoldSupport = value; }
		}

		/// <summary>
		/// Gets or sets the JSON serializer.
		/// </summary>
		/// <value>The JSON serializer.</value>
		public IJSONSerializer JSONSerializer
		{
			get { return jsonSerializer; }
			set { jsonSerializer = value; }
		}

		/// <summary>
		/// Gets or sets the static resource registry service.
		/// </summary>
		/// <value>The static resource registry.</value>
		public IStaticResourceRegistry StaticResourceRegistry
		{
			get { return staticResourceRegistry; }
			set { staticResourceRegistry = value; }
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>()
		{
			return (T) service2Impl[typeof(T)];
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>
		/// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
		/// </returns>
		public object GetService(Type serviceType)
		{
			return service2Impl[serviceType];
		}
	}
}