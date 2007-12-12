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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web;
	using System.Web.UI;
	using System.IO;
	using System.Collections;
	using System.Reflection;
	using JSGeneration;

	/// <summary>
	/// Default implementation of a <see cref="IViewEngine"/>.
	/// Uses ASP.Net WebForms as views.
	/// </summary>
	public class WebFormsViewEngine : ViewEngineBase
	{
		private static readonly String ProcessedBeforeKey = "processed_before";

		private static readonly BindingFlags PropertyBindingFlags = BindingFlags.Public |
		                                                            BindingFlags.NonPublic | BindingFlags.Instance |
		                                                            BindingFlags.IgnoreCase;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebFormsViewEngine"/> class.
		/// </summary>
		public WebFormsViewEngine()
		{
		}

		#region ViewEngineBase overrides

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public override string ViewFileExtension
		{
			get { return ".aspx"; }
		}

		/// <summary>
		/// Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public override string JSGeneratorFileExtension
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public override bool HasTemplate(String templateName)
		{
			return ViewSourceLoader.HasSource(templateName + ".aspx");
		}

		/// <summary>
		/// Obtains the aspx Page from the view name dispatch
		/// its execution using the standard ASP.Net API.
		/// </summary>
		public override void Process(String viewName, TextWriter output, IEngineContext context, IController controller,
		                             IControllerContext controllerContext)
		{
			AdjustContentType(context);

			bool processedBefore = false;

			//TODO: document this hack for the sake of our users
			HttpContext httpContext = context.UnderlyingContext;

			if (httpContext != null)
			{
				if (!(processedBefore = httpContext.Items.Contains(ProcessedBeforeKey)))
				{
					httpContext.Items[ProcessedBeforeKey] = true;
				}
			}

			if (processedBefore)
			{
#if !MONO
				ProcessExecuteView(context, controller, controllerContext, viewName);
				return;
#else
				if (IsTheSameView(httpContext, viewName)) return;
#endif
			}

			ProcessInlineView(controller, controllerContext, viewName, httpContext);
		}

		/// <summary>
		/// Processes the partial.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="partialName">The partial name.</param>
		public override void ProcessPartial(string partialName, TextWriter output, 
			IEngineContext context, IController controller,
		                                    IControllerContext controllerContext
		                                    )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates the JS generator.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public override IJSGenerator CreateJSGenerator(IEngineContext context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Generates the JS.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="templateName">Name of the template.</param>
		public override void GenerateJS(string templateName, TextWriter output, IEngineContext context, IController controller,
		                                IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Wraps the specified content in the layout using the
		/// context to output the result.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="contents">The contents.</param>
		public override void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
		                                     IControllerContext controllerContext)
		{
			AdjustContentType(context);

			HttpContext httpContext = context.UnderlyingContext;

			Page masterHandler = ObtainMasterPage(httpContext, controllerContext);

			httpContext.Items.Add("rails.contents", contents);

			ProcessPage(controller, masterHandler, httpContext);
		}

		#endregion

		private void ProcessInlineView(IController controller, IControllerContext controllerContext, String viewName,
		                               HttpContext httpContext)
		{
			PrepareLayout(controllerContext, httpContext);

			IHttpHandler childPage = GetCompiledPageInstance(viewName, httpContext);

			ProcessPropertyBag(controllerContext.PropertyBag, childPage);

#if !MONO
			Page page = childPage as Page;

			if (page != null)
			{
				page.Init += (PrepareMasterPage);
			}
#endif

			ProcessPage(controller, childPage, httpContext);

			ProcessLayoutIfNeeded(controller, controllerContext, httpContext, childPage);
		}

#if !MONO
		private void ProcessExecuteView(IEngineContext context, IController controller, IControllerContext controllerContext, String viewName)
		{
			HttpContext httpContext = context.UnderlyingContext;

			PrepareLayout(controllerContext, httpContext);

			ExecutePageProvider pageProvider = new ExecutePageProvider(this, viewName);

			Page childPage = pageProvider.ExecutePage(context);

			ProcessLayoutIfNeeded(controller, controllerContext, httpContext, childPage);

			// Prevent the parent Page from continuing to process.
			httpContext.Response.End();
		}

		private void PrepareView(object sender, EventArgs e)
		{
			Page view = (Page) sender;

			IController controller = GetCurrentController();
			IControllerContext controllerContext = GetCurrentControllerContext();

			ProcessPropertyBag(controllerContext.PropertyBag, view);

			PreSendView(controller, view);

			PrepareMasterPage(sender, e);

			view.Unload += (FinalizeView);
		}

		private void PrepareMasterPage(object sender, EventArgs e)
		{
			Page view = (Page) sender;
			MasterPage masterPage = view.Master;

			if (masterPage != null)
			{
				IController controller = GetCurrentController();
				IControllerContext controllerContext = GetCurrentControllerContext();

				if (masterPage is IControllerAware)
				{
					(masterPage as IControllerAware).SetController(controller, controllerContext);
				}
			}
		}

		private void FinalizeView(object sender, EventArgs e)
		{
			IController controller = GetCurrentController();

			PostSendView(controller, sender);
		}
#endif

		private String MapViewToPhysicalPath(String viewName)
		{
			viewName += ".aspx";

			// TODO: There must be a more efficient way to do this than two replace operations
			return
				Path.Combine(ViewSourceLoader.ViewRootDir, viewName).
				Replace('/', Path.DirectorySeparatorChar).
				Replace('\\', Path.DirectorySeparatorChar);
		}

		private String MapViewToVirtualPath(String viewName, ref string physicalPath, HttpContext httpContext)
		{
			if (physicalPath == null)
			{
				physicalPath = MapViewToPhysicalPath(viewName);
			}

			String physicalAppPath = httpContext.Request.PhysicalApplicationPath;
			if (physicalPath.StartsWith(physicalAppPath))
			{
				viewName = "~/" + physicalPath.Substring(physicalAppPath.Length);
			}

			return viewName;
		}

		private IHttpHandler GetCompiledPageInstance(String viewName, HttpContext httpContext)
		{
			String physicalPath = null;

			// This is a hack until I can understand the different behavior exhibited by
			// PageParser.GetCompiledPageInstance(..) when running ASP.NET 2.0.  It appears
			// that the virtualPath (first argument) to this method must represent a valid
			// virtual directory with respect to the web application.   As a result, the
			// viewRootDir must be relative to the web application project.  If it is not,
			// this will certainly fail.  If this is indeed the case, it may make sense to
			// be able to obtain an absolute virtual path to the views directory from the
			// ViewSourceLoader.

			viewName = MapViewToVirtualPath(viewName, ref physicalPath, httpContext);

			return PageParser.GetCompiledPageInstance(viewName, physicalPath, httpContext);
		}

#if MONO
		private bool IsTheSameView(HttpContext httpContext, String viewName)
		{
			String original = ((String) httpContext.Items[Constants.OriginalViewKey]);
			String actual = viewName;

			return String.Compare(original, actual, true) == 0;
		}
#endif

		private void ProcessPage(IController controller, IHttpHandler page, HttpContext httpContext)
		{
			PreSendView(controller, page);

			page.ProcessRequest(httpContext);

			PostSendView(controller, page);
		}

		private void PrepareLayout(IControllerContext controller, HttpContext httpContext)
		{
			if (HasLayout(controller))
			{
				bool layoutPending = httpContext.Items.Contains("wfv.masterPage");

				Page masterHandler = ObtainMasterPage(httpContext, controller);

				if (!layoutPending && masterHandler != null)
				{
					StartFiltering(httpContext.Response);
				}
			}
		}

		private bool ProcessLayoutIfNeeded(IController controller, IControllerContext controllerContext, HttpContext httpContext, IHttpHandler childPage)
		{
			Page masterHandler = (Page) httpContext.Items["wfv.masterPage"];

			if (masterHandler != null && HasLayout(controllerContext))
			{
				byte[] contents = RestoreFilter(httpContext.Response);

				// Checks if its only returning from a inner process invocation
				if (!Convert.ToBoolean(httpContext.Items["rails.layout.processed"]))
				{
					httpContext.Items.Add("rails.contents", contents);
					httpContext.Items.Add("rails.child", childPage);

					httpContext.Items["rails.layout.processed"] = true;
				}

				ProcessPage(controller, masterHandler, httpContext);

				return true;
			}

			return false;
		}

		private bool HasLayout(IControllerContext controllerContext)
		{
			return controllerContext.LayoutName != null;
		}

//		private void WriteBuffered(HttpResponse response)
//		{
//			// response.Flush();
//
//			// Restores the original stream
//			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
//			response.Filter = filter.OriginalStream;
//			
//			// Writes the buffered contents
//			byte[] buffer = filter.GetBuffer();
//			response.OutputStream.Write(buffer, 0, buffer.Length);
//		}

		private byte[] RestoreFilter(HttpResponse response)
		{
			response.Flush();
			response.Filter.Flush();

			// Restores the original stream
			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
			response.Filter = filter.OriginalStream;

			return filter.GetBuffer();
		}

		private Page ObtainMasterPage(HttpContext httpContext, IControllerContext controller)
		{
			Page masterHandler;
			String layout = "layouts/" + controller.LayoutName;

#if !MONO
			masterHandler = (Page) httpContext.Items["wfv.masterPage"];

			if (masterHandler != null)
			{
				String currentLayout = (String) masterHandler.Items["wfv.masterLayout"];
				if (layout == currentLayout) return masterHandler;
			}
#endif
			masterHandler = (Page) GetCompiledPageInstance(layout, httpContext);
#if !MONO
			masterHandler.Items["wfv.masterLayout"] = layout;
#endif

			httpContext.Items["wfv.masterPage"] = masterHandler;
			return masterHandler;
		}

		private void StartFiltering(HttpResponse response)
		{
			Stream filter = response.Filter;
			response.Filter = new DelegateMemoryStream(filter);
			response.BufferOutput = true;
		}

		private void ProcessPropertyBag(IDictionary bag, IHttpHandler handler)
		{
			foreach(DictionaryEntry entry in bag)
			{
				SetPropertyValue(handler, entry.Key, entry.Value);
			}
		}

		private void SetPropertyValue(IHttpHandler handler, object key, object value)
		{
			if (value == null) return;

			String name = key.ToString();
			Type type = handler.GetType();
			Type valueType = value.GetType();

			FieldInfo fieldInfo = type.GetField(name, PropertyBindingFlags);

			if (fieldInfo != null)
			{
				if (fieldInfo.FieldType.IsAssignableFrom(valueType))
				{
					fieldInfo.SetValue(handler, value);
				}
			}
			else
			{
				PropertyInfo propInfo = type.GetProperty(name, PropertyBindingFlags);

				if (propInfo != null && (propInfo.CanWrite &&
				                         (propInfo.PropertyType.IsAssignableFrom(valueType))))
				{
					propInfo.GetSetMethod().Invoke(handler, new object[] {value});
				}
			}
		}

#if !MONO

		private IController GetCurrentController()
		{
			return MonoRailHttpHandlerFactory.CurrentController;
		}

		private IControllerContext GetCurrentControllerContext()
		{
			return MonoRailHttpHandlerFactory.CurrentControllerContext;
		}

		internal class ExecutePageProvider : IMonoRailHttpHandlerProvider
		{
			private Page page;
			private string viewName;
			private WebFormsViewEngine engine;

			/// <summary>
			/// Initializes a new instance of the <see cref="ExecutePageProvider"/> class.
			/// </summary>
			/// <param name="engine">The engine.</param>
			/// <param name="viewName">Name of the view.</param>
			public ExecutePageProvider(WebFormsViewEngine engine, string viewName)
			{
				this.page = null;
				this.engine = engine;
				this.viewName = viewName;
			}

			/// <summary>
			/// Executes the page.
			/// </summary>
			/// <param name="context">The context.</param>
			/// <returns></returns>
			public Page ExecutePage(IEngineContext context)
			{
				HttpContext httpContext = context.UnderlyingContext;
				context.RemoveService(typeof(IMonoRailHttpHandlerProvider));
				context.AddService(typeof(IMonoRailHttpHandlerProvider), this);
				httpContext.Server.Execute(httpContext.Request.RawUrl, false);
				return page;
			}

			/// <summary>
			/// Implementors should perform their logic to
			/// return a instance of <see cref="IHttpHandler"/>.
			/// If the <see cref="IHttpHandler"/> can not be created,
			/// it should return <c>null</c>.
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public IHttpHandler ObtainMonoRailHttpHandler(IEngineContext context)
			{
				HttpContext httpContext = context.UnderlyingContext;

				string physicalPath = null;
				string virtualPath = engine.MapViewToVirtualPath(
					viewName, ref physicalPath, httpContext);

				if (virtualPath != null && physicalPath != null)
				{
					page = (Page) PageParser.GetCompiledPageInstance(
					              	virtualPath, physicalPath, httpContext);

					if (page != null)
					{
						page.Init += new EventHandler(engine.PrepareView);
					}
				}

				return page;
			}

			/// <summary>
			/// Implementors should perform their logic
			/// to release the <see cref="IHttpHandler"/> instance
			/// and its resources.
			/// </summary>
			/// <param name="handler"></param>
			public void ReleaseHandler(IHttpHandler handler)
			{
			}
		}
#endif
	}


	internal class DelegateMemoryStream : MemoryStream
	{
		public Stream _original;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateMemoryStream"/> class.
		/// </summary>
		/// <param name="original">The original.</param>
		public DelegateMemoryStream(Stream original)
		{
			_original = original;
		}

		/// <summary>
		/// Gets the original stream.
		/// </summary>
		/// <value>The original stream.</value>
		public Stream OriginalStream
		{
			get { return _original; }
		}
	}
}