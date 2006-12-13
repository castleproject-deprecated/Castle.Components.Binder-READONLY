namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultViewEngineManager : IViewEngineManager, IServiceEnabledComponent, IInitializable
	{
		private MonoRailConfiguration config;
		private IServiceProvider provider;
		private IDictionary ext2ViewEngine;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultViewEngineManager"/> class.
		/// </summary>
		public DefaultViewEngineManager()
		{
			ext2ViewEngine = new HybridDictionary(true);
		}

		#region IInitializable
		
		public void Initialize()
		{
			foreach(ViewEngineInfo info in config.ViewEngineConfig.ViewEngines)
			{
				try
				{
					IViewEngine engine = (IViewEngine) Activator.CreateInstance(info.Engine);

					RegisterEngineForView(engine);

					engine.XHtmlRendering = info.XhtmlRendering;

					IServiceEnabledComponent serviceEnabled = engine as IServiceEnabledComponent;
					
					if (serviceEnabled != null)
					{
						serviceEnabled.Service(provider);
					}

					IInitializable initializable = engine as IInitializable;

					if (initializable != null)
					{
						initializable.Initialize();
					}
				}
				catch(InvalidCastException)
				{
					throw new RailsException("Type " + info.Engine.FullName + " does not implement IViewEngine");
				}
				catch(Exception ex)
				{
					throw new RailsException("Could not create view engine instance: " + info.Engine, ex);
				}
			}

			config = null;
		}

		#endregion

		#region IServiceEnabledComponent

		public void Service(IServiceProvider provider)
		{
			this.provider = provider;
			
			config = (MonoRailConfiguration) provider.GetService(typeof(MonoRailConfiguration));
		}

		#endregion

		#region IViewEngineManager

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName">View template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			return ResolveEngine(templateName, false) != null;
		}

		public void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			IViewEngine engine = ResolveEngine(templateName);
			
			engine.Process(context, controller, templateName);
		}

		public void Process(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			IViewEngine engine = ResolveEngine(templateName);

			engine.Process(output, context, controller, templateName);
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			if (controller.LayoutName == null)
			{
				throw new RailsException("ProcessContents can only work with a layout");
			}

			IViewEngine engine = ResolveEngine(Path.Combine("layouts", controller.LayoutName));

			engine.ProcessContents(context, controller, contents);
		}

		#endregion

		private IViewEngine ResolveEngine(string viewName)
		{
			return ResolveEngine(viewName, true);
		}
		
		/// <summary>
		/// The view can be informed with an extension. If so, we use it
		/// to discover the extension. Otherwise, we use the view source
		/// to find out the file that exists there, and hence the view 
		/// engine instance
		/// </summary>
		/// <param name="viewName">View name</param>
		/// <param name="throwException"><c>true</c> if an exception should be threw if no match is found</param>
		/// <returns>A view engine instance</returns>
		private IViewEngine ResolveEngine(string viewName, bool throwException)
		{
			if (Path.HasExtension(viewName))
			{
				String extension = Path.GetExtension(viewName);
				
				IViewEngine engine = (IViewEngine) ext2ViewEngine[extension];

				if (engine != null) return engine;

				if (!throwException) return null;

				throw new RailsException(
					"Could not find a suitable View engine to " +
					"handle view template with extension " + extension + ". View template: " + viewName);
			}
			else
			{
				foreach(IViewEngine engine in ext2ViewEngine.Values)
				{
					if (engine.HasTemplate(viewName))
					{
						return engine;
					}
				}

				if (!throwException) return null;

				throw new RailsException(
					"Could not find a suitable View engine to " +
					"handle view template or maybe there " + 
					"are no views on the folder that matches the name. View template: " + viewName);

			}
		}

		/// <summary>
		/// Associates extensions with the view engine instance.
		/// </summary>
		/// <param name="engine">The view engine instance</param>
		private void RegisterEngineForView(IViewEngine engine)
		{
			if (ext2ViewEngine.Contains(engine.ViewFileExtension))
			{
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.ViewFileExtension];
				
				throw new RailsException(
					"At least two view engines are handling the same file extension. " + 
					"This isn't going to work. Extension: " + engine.ViewFileExtension + 
					" View Engine 1: " + existing.GetType() + 
					" View Engine 2: " + engine.GetType());
			}

			String extension = engine.ViewFileExtension.StartsWith(".")
			                   	? engine.ViewFileExtension
			                   	: "." + engine.ViewFileExtension;

			ext2ViewEngine[extension] = engine;
			
			if (engine.SupportsJSGeneration && ext2ViewEngine.Contains(engine.JSGeneratorFileExtension))
			{
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.JSGeneratorFileExtension];

				throw new RailsException(
					"At least two view engines are handling the same file extension. " +
					"This isn't going to work. Extension: " + engine.JSGeneratorFileExtension +
					" View Engine 1: " + existing.GetType() +
					" View Engine 2: " + engine.GetType());
			}
						
			if (engine.SupportsJSGeneration)
			{
				extension = engine.JSGeneratorFileExtension.StartsWith(".")
								? engine.JSGeneratorFileExtension
								: "." + engine.JSGeneratorFileExtension;

				ext2ViewEngine[extension] = engine;
			}
		}
	}
}
