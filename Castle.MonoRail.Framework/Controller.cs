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
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Web;
	using Castle.Components.Validator;
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Resources;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Implements the core functionality and exposes the
	/// common methods for concrete controllers.
	/// </summary>
	public abstract class Controller : IController
	{
		private IEngineContext engineContext;
		private IControllerContext context;
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// True if any Controller.Send operation was called.
		/// </summary>
		private bool resetIsPostBack;
		private bool directRenderInvoked;

		private IUrlBuilder urlBuilder;
		private IFilterFactory filterFactory;
		private IViewEngineManager viewEngineManager;
		private IActionSelector actionSelector;
		private ValidatorRunner validator;
		private FilterDescriptor[] filters;
		private IDictionary filtersToSkip = new HybridDictionary();
//		private ValidatorRunner validatorRunner;

		#region Useful Properties

		/// <summary>
		/// Gets the view folder -- (areaname + 
		/// controllername) or just controller name -- that this controller 
		/// will use by default.
		/// </summary>
		public string ViewFolder
		{
			get { return context.ViewFolder; }
			set { context.ViewFolder = value; }
		}

		/// <summary>
		/// This is intended to be used by MonoRail infrastructure.
		/// </summary>
		public ControllerMetaDescriptor MetaDescriptor
		{
			get { return context.ControllerDescriptor; }
			set { context.ControllerDescriptor = value; }
		}

		/// <summary>
		/// Gets the actions available in this controller.
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The actions.</value>
		public ICollection Actions
		{
			get { return MetaDescriptor.Actions.Values; }
		}

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The resources.</value>
		public ResourceDictionary Resources
		{
			get { return context.Resources; }
		}

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		public IDictionary Helpers
		{
			get { return context.Helpers; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a post.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a post; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPost
		{
			get { return engineContext.Request.HttpMethod == "POST"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a get.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a get; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsGet
		{
			get { return engineContext.Request.HttpMethod == "GET"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a put.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a put; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPut
		{
			get { return engineContext.Request.HttpMethod == "PUT"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a head.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a head; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsHead
		{
			get { return engineContext.Request.HttpMethod == "HEAD"; }
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		public string Name
		{
			get { return context.Name; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		public string AreaName
		{
			get { return context.AreaName; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		public string LayoutName
		{
			get { return context.LayoutName; }
			set { context.LayoutName = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		public string Action
		{
			get { return context.Action; }
		}

		/// <summary>
		/// Logger for the controller
		/// </summary>
		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		/// <summary>
		/// Gets or sets the view which will be rendered by this action.
		/// </summary>
		public string SelectedViewName
		{
			get { return context.SelectedViewName; }
			set { context.SelectedViewName = value; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return context.PropertyBag; }
			set { context.PropertyBag = value; }
		}

		/// <summary>
		/// Gets the context of this request execution.
		/// </summary>
		public IEngineContext Context
		{
			get { return engineContext; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return engineContext.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		public Flash Flash
		{
			get { return engineContext.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected internal HttpContext HttpContext
		{
			get { return engineContext.UnderlyingContext; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		public IRequest Request
		{
			get { return Context.Request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		public IResponse Response
		{
			get { return Context.Response; }
		}

		/// <summary>
		/// Shortcut to <see cref="IRequest.Params"/> 
		/// </summary>
		public NameValueCollection Params
		{
			get { return Request.Params; }
		}

		/// <summary>
		/// Shortcut to <see cref="IRequest.Form"/> 
		/// </summary>
		public NameValueCollection Form
		{
			get { return Request.Form; }
		}

		/// <summary>
		/// Shortcut to <see cref="IRequest.QueryString"></see>
		/// </summary>
		public NameValueCollection Query
		{
			get { return Request.QueryString; }
		}

		/// <summary>
		/// Gets the dynamic actions dictionary. 
		/// <para>
		/// Can be used to insert dynamic actions on the controller instance.
		/// </para>
		/// </summary>
		/// <value>The dynamic actions dictionary.</value>
		public IDictionary<string, IDynamicAction> DynamicActions
		{
			get { return context.DynamicActions; }
		}

		/// <summary>
		/// Gets the validator runner instance.
		/// </summary>
		/// <value>The validator instance.</value>
		public ValidatorRunner Validator
		{
			get { return validator; }
			set { validator = value; }
		}

		/// <summary>
		/// Gets the URL builder instance.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
		}

		/// <summary>
		/// Shortcut to 
		/// <see cref="IResponse.IsClientConnected"/>
		/// </summary>
		protected bool IsClientConnected
		{
			get { return engineContext.Response.IsClientConnected; }
		}

		/// <summary>
		/// Indicates that the current Action resulted from an ASP.NET PostBack.
		/// As a result, this property is only relavent to controllers using 
		/// WebForms views.  It is placed on the base Controller for convenience 
		/// only to avoid the need to extend the Controller or provide additional 
		/// helper classes.  It is marked virtual to better support testing.
		/// </summary>
		protected virtual bool IsPostBack
		{
			get
			{
				if (resetIsPostBack) return false;

				NameValueCollection fields = Params;
				return (fields["__VIEWSTATE"] != null) || (fields["__EVENTTARGET"] != null);
			}
		}

		#endregion

		#region Useful Operations

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string name)
		{
			context.SelectedViewName = Path.Combine(ViewFolder, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();
			Response.ContentType = mimeType;

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string controller, string name)
		{
			context.SelectedViewName = Path.Combine(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string controller, string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();

			Response.ContentType = mimeType;
			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, string mimeType)
		{
			Response.ContentType = mimeType;
			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed and results are written to System.IO.TextWriter. 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderView(TextWriter output, string name)
		{
			viewEngineManager.Process(output, Context, this, context, Path.Combine(ViewFolder, name));
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name)
		{
			context.SelectedViewName = name;
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderSharedView(name);
		}

		/// <summary>
		/// Specifies the shared view to be processed and results are written to System.IO.TextWriter.
		/// (A partial view shared by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderSharedView(TextWriter output, string name)
		{
			viewEngineManager.Process(output, Context, this, context, name);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public void CancelView()
		{
			context.SelectedViewName = null;
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public void CancelLayout()
		{
			LayoutName = null;
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents)
		{
			CancelView();

			engineContext.Response.Write(contents);
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents, params object[] args)
		{
			RenderText(String.Format(contents, args));
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(IFormatProvider formatProvider, string contents, params object[] args)
		{
			RenderText(String.Format(formatProvider, contents, args));
		}

		/// <summary>
		/// Sends raw contents to be rendered directly by the view engine.
		/// It's up to the view engine just to apply the layout and nothing else.
		/// </summary>
		/// <param name="contents">Contents to be rendered.</param>
		public void DirectRender(string contents)
		{
			CancelView();

			if (directRenderInvoked)
			{
				throw new ControllerException("DirectRender should be called only once.");
			}

			directRenderInvoked = true;

			viewEngineManager.ProcessContents(engineContext, this, context, contents);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(string templateName)
		{
			return viewEngineManager.HasTemplate(templateName);
		}

		#region RedirectToRoute

		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		public void RedirectToRoute(string routeName)
		{
			Redirect(UrlBuilder.BuildRouteUrl(Context.UrlInfo, routeName));
		}

		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="parameters">The parameters.</param>
		public void RedirectToRoute(string routeName, IDictionary parameters)
		{
			Redirect(UrlBuilder.BuildRouteUrl(Context.UrlInfo, routeName, parameters));
		}

		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="parameters">The parameters.</param>
		public void RedirectToRoute(string routeName, object parameters)
		{
			Redirect(UrlBuilder.BuildRouteUrl(Context.UrlInfo, routeName, parameters));
		}

		#endregion

		#region RedirectToAction

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		protected void RedirectToAction(string action)
		{
			RedirectToAction(action, (NameValueCollection) null);
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">list of key/value pairs. Each string is supposed
		/// to have the format "key=value" that will be converted to a proper 
		/// query string</param>
		protected void RedirectToAction(string action, params String[] queryStringParameters)
		{
			RedirectToAction(action, DictHelper.Create(queryStringParameters));
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">Query string entries</param>
		protected void RedirectToAction(string action, IDictionary queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">Query string entries</param>
		protected void RedirectToAction(string action, NameValueCollection queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		#endregion

		/// <summary>
		/// Redirects to the referrer action, according to the "HTTP_REFERER" header (<c>Context.UrlReferrer</c>).
		/// </summary>
		[Obsolete("Use RedirectToReferrer")]
		protected void RedirectToReferer()
		{
			RedirectToReferrer();
		}

		/// <summary>
		/// Redirects to the referrer action, according to the "HTTP_REFERER" header (<c>Context.UrlReferrer</c>).
		/// </summary>
		protected void RedirectToReferrer()
		{
			Redirect(Context.Request.UrlReferrer);
		}

		/// <summary> 
		/// Redirects to the site root directory (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		public void RedirectToSiteRoot()
		{
			Redirect(Context.ApplicationPath + "/");
		}

		/// <summary>
		/// Redirects to the specified URL. All other Redirects call this one.
		/// </summary>
		/// <param name="url">Target URL</param>
		public virtual void Redirect(string url)
		{
			CancelView();

			Context.Response.RedirectToUrl(url);
		}

		/// <summary>
		/// Redirects to the specified URL. 
		/// </summary>
		/// <param name="url">Target URL</param>
		/// <param name="parameters">URL parameters</param>
		public virtual void Redirect(string url, IDictionary parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url = url + '&' + ToQueryString(parameters);
				}
				else
				{
					url = url + '?' + ToQueryString(parameters);
				}
			}

			Redirect(url);
		}

		/// <summary>
		/// Redirects to the specified URL. 
		/// </summary>
		/// <param name="url">Target URL</param>
		/// <param name="parameters">URL parameters</param>
		public virtual void Redirect(string url, NameValueCollection parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url = url + '&' + ToQueryString(parameters);
				}
				else
				{
					url = url + '?' + ToQueryString(parameters);
				}
			}

			Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(string controller, string action)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action));
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(string area, string controller, string action)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, NameValueCollection parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, NameValueCollection parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, IDictionary parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, IDictionary parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action, parameters));
		}

		/// <summary>
		/// Creates a querystring string representation of the namevalue collection.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected string ToQueryString(NameValueCollection parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		/// <summary>
		/// Creates a querystring string representation of the entries in the dictionary.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected string ToQueryString(IDictionary parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		#endregion

		#region IController

		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		public virtual void Process(IEngineContext engineContext, IControllerContext context)
		{
			this.engineContext = engineContext;
			this.context = context;

			urlBuilder = engineContext.Services.UrlBuilder;
			filterFactory = engineContext.Services.FilterFactory;
			viewEngineManager = engineContext.Services.ViewEngineManager;
			actionSelector = engineContext.Services.ActionSelector;
//			validatorRunner = CreateValidatorRunner(engineContext.Services.ValidatorRegistry);

			ResetIsPostback();
			context.LayoutName = ObtainDefaultLayoutName();
			CreateAndInitializeHelpers();
			CreateFiltersDescriptors();
			// ProcessScaffoldIfPresent();
			// ActionProviderUtil.RegisterActions(controller);

			Initialize();

			RunActionAndRenderView();
		}

		private void RunActionAndRenderView()
		{
			IExecutableAction action = SelectAction(Action);

			bool cancel;
			RunBeforeActionFilters(action, out cancel);
			if (cancel) return;

			Exception actionException = null;

			try
			{
				action.Execute(engineContext, this, context);
			}
			catch(Exception ex)
			{
				actionException = ex;
			}

			RunAfterActionFilters(action, out cancel);
			if (cancel) return;

			if (actionException == null)
			{
				if (context.SelectedViewName != null)
				{
					ProcessView();
				}
			}
			else
			{
				ProcessRescue();
			}

			RunAfterRenderingFilters(action);
		}

		protected virtual IExecutableAction SelectAction(string action)
		{
			// For backward compatibility purposes
			MethodInfo method = SelectMethod(action, MetaDescriptor.Actions, engineContext.Request, null);

			if (method != null)
			{
				ActionMetaDescriptor actionMeta = MetaDescriptor.GetAction(method);

				return new ActionMethodExecutorCompatible(method, actionMeta, InvokeMethod);
			}

			// New supported way
			return actionSelector.Select(engineContext, this, context);
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PreSendView(object view)
		{
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PostSendView(object view)
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			DisposeFilters();
		}

		#endregion

		#region Lifecycle (overridables)

		/// <summary>
		/// Initializes this instance. Implementors 
		/// can use this method to perform initialization
		/// </summary>
		protected virtual void Initialize()
		{
		}

		#endregion

		#region Views and Layout

		/// <summary>
		/// Obtains the name of the default layout.
		/// </summary>
		/// <returns></returns>
		protected virtual String ObtainDefaultLayoutName()
		{
			if (MetaDescriptor.Layout != null)
			{
				return MetaDescriptor.Layout.LayoutName;
			}
			else
			{
				String defaultLayout = String.Format("layouts/{0}", Name);

				if (viewEngineManager.HasTemplate(defaultLayout))
				{
					return Name;
				}
			}

			return null;
		}

		/// <summary>
		/// Processes the view.
		/// </summary>
		protected virtual void ProcessView()
		{
			if (context.SelectedViewName != null)
			{
				viewEngineManager.Process(engineContext.Response.Output, engineContext, this, context, context.SelectedViewName);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Creates the and initialize helpers associated with a controller.
		/// </summary>
		protected virtual void CreateAndInitializeHelpers()
		{
			IDictionary helpers = context.Helpers;

			// Custom helpers

			foreach(HelperDescriptor helper in MetaDescriptor.Helpers)
			{
				object helperInstance = Activator.CreateInstance(helper.HelperType);

				PerformHelperInitialization(helperInstance);

				if (helpers.Contains(helper.Name))
				{
					throw new ControllerException(String.Format("Found a duplicate helper " +
					                                            "attribute named '{0}' on controller '{1}'", helper.Name, Name));
				}

				helpers.Add(helper.Name, helperInstance);
			}

			CreateStandardHelpers(helpers);
		}

		/// <summary>
		/// Creates the standard helpers.
		/// </summary>
		/// <param name="helpers">The helpers.</param>
		protected virtual void CreateStandardHelpers(IDictionary helpers)
		{
			AbstractHelper[] builtInHelpers =
				new AbstractHelper[]
					{
						new AjaxHelper(), //  new BehaviourHelper(),
						new UrlHelper(), // new TextHelper(), 
						// new EffectsFatHelper(), new ScriptaculousHelper(), 
						// new DateFormatHelper(), new HtmlHelper(),
						// new ValidationHelper(), new DictHelper(),
						// new PaginationHelper(), new FormHelper(),
						// new ZebdaHelper()
					};

			foreach(AbstractHelper helper in builtInHelpers)
			{
				helper.SetController(this, context);
				helper.SetContext(engineContext);

				string helperName = helper.GetType().Name;

				if (!helpers.Contains(helperName))
				{
					helpers[helperName] = helper;
				}

				// Also makes the helper available with a less verbose name
				// FormHelper and Form, AjaxHelper and Ajax
				if (helperName.EndsWith("Helper"))
				{
					helpers[helperName.Substring(0, helperName.Length - 6)] = helper;
				}

				PerformHelperInitialization(helper);
			}
		}

		/// <summary>
		/// Performs the additional helper initialization
		/// checking if the helper instance implements <see cref="IServiceEnabledComponent"/>.
		/// </summary>
		/// <param name="helperInstance">The helper instance.</param>
		private void PerformHelperInitialization(object helperInstance)
		{
			IControllerAware aware = helperInstance as IControllerAware;

			if (aware != null)
			{
				aware.SetController(this, context);
			}

			IServiceEnabledComponent serviceEnabled = helperInstance as IServiceEnabledComponent;

			if (serviceEnabled != null)
			{
				serviceEnabled.Service(engineContext);
			}

			IMRServiceEnabled mrServiceEnabled = helperInstance as IMRServiceEnabled;

			if (mrServiceEnabled != null)
			{
				mrServiceEnabled.Service(engineContext.Services);
			}
		}

		#endregion

		#region Filters

		private void CreateFiltersDescriptors()
		{
			if (MetaDescriptor.Filters.Length != 0)
			{
				filters = CopyFilterDescriptors();
			}
		}

		private void RunBeforeActionFilters(IExecutableAction action, out bool cancel)
		{
			cancel = false;
			if (action.ShouldSkipAllFilters) return;

			if (!ProcessFilters(action, ExecuteEnum.BeforeAction))
			{
				cancel = true;
				return; // A filter returned false... so we stop
			}
		}

		private void RunAfterActionFilters(IExecutableAction action, out bool cancel)
		{
			cancel = false;
			if (action.ShouldSkipAllFilters) return;

			if (!ProcessFilters(action, ExecuteEnum.AfterAction))
			{
				cancel = true;
				return; // A filter returned false... so we stop
			}
		}

		private void RunAfterRenderingFilters(IExecutableAction action)
		{
			if (action.ShouldSkipAllFilters) return;

			ProcessFilters(action, ExecuteEnum.AfterRendering);
		}

		/// <summary>
		/// Identifies if no filter should run for the given action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected virtual bool ShouldSkipFilters(IExecutableAction action)
		{
			if (filters == null)
			{
				// No filters, so skip 
				return true;
			}

			return action.ShouldSkipAllFilters;

//			ActionMetaDescriptor actionMeta = MetaDescriptor.GetAction(method);
//
//			if (actionMeta.SkipFilters.Count == 0)
//			{
//				// Nothing against filters declared for this action
//				return false;
//			}
//
//			foreach(SkipFilterAttribute skipfilter in actionMeta.SkipFilters)
//			{
//				// SkipAllFilters handling...
//				if (skipfilter.BlanketSkip)
//				{
//					return true;
//				}
//
//				filtersToSkip[skipfilter.FilterType] = String.Empty;
//			}
//
//			return false;
		}

		/// <summary>
		/// Clones all Filter descriptors, in order to get a writable copy.
		/// </summary>
		protected internal FilterDescriptor[] CopyFilterDescriptors()
		{
			FilterDescriptor[] clone = (FilterDescriptor[]) MetaDescriptor.Filters.Clone();

			for(int i = 0; i < clone.Length; i++)
			{
				clone[i] = (FilterDescriptor) clone[i].Clone();
			}

			return clone;
		}

		private bool ProcessFilters(IExecutableAction action, ExecuteEnum when)
		{
			foreach(FilterDescriptor desc in filters)
			{
				if (action.ShouldSkipFilter(desc.FilterType))
				{
					continue;
				}

				if ((desc.When & when) != 0)
				{
					if (!ProcessFilter(when, desc))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool ProcessFilter(ExecuteEnum when, FilterDescriptor desc)
		{
			if (desc.FilterInstance == null)
			{
				desc.FilterInstance = filterFactory.Create(desc.FilterType);

				IFilterAttributeAware filterAttAware = desc.FilterInstance as IFilterAttributeAware;

				if (filterAttAware != null)
				{
					filterAttAware.Filter = desc.Attribute;
				}
			}

			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat("Running filter {0}/{1}", when, desc.FilterType.FullName);
				}

				return desc.FilterInstance.Perform(when, engineContext, this, context);
			}
			catch(Exception ex)
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat("Error processing filter " + desc.FilterType.FullName, ex);
				}

				throw;
			}
		}

		private void DisposeFilters()
		{
			if (filters == null) return;

			foreach(FilterDescriptor desc in filters)
			{
				if (desc.FilterInstance != null)
				{
					filterFactory.Release(desc.FilterInstance);
				}
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
//			context.LastException = (ex is TargetInvocationException) ? ex.InnerException : ex;
//
//			Type exceptionType = context.LastException.GetType();
//
//			RescueDescriptor att = null;
//
//			if (method != null)
//			{
//				ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);
//
//				if (actionMeta.SkipRescue != null) return false;
//
//				att = GetRescueFor(actionMeta.Rescues, exceptionType);
//			}
//
//			if (att == null)
//			{
//				att = GetRescueFor(metaDescriptor.Rescues, exceptionType);
//
//				if (att == null) return false;
//			}
//
//			try
//			{
//				if (att.RescueController != null)
//				{
//					Controller rescueController = (Controller)Activator.CreateInstance(att.RescueController);
//
//					using (ControllerLifecycleExecutor rescueExecutor = new ControllerLifecycleExecutor(rescueController, context))
//					{
//						rescueExecutor.Service(provider);
//						ControllerDescriptor rescueDescriptor = ControllerInspectionUtil.Inspect(att.RescueController);
//						rescueExecutor.InitializeController(rescueDescriptor.Area, rescueDescriptor.Name, att.RescueMethod.Name);
//						rescueExecutor.SelectAction(att.RescueMethod.Name, rescueDescriptor.Name);
//
//						IDictionary args = new Hashtable();
//						IDictionary propertyBag = rescueController.PropertyBag;
//						args["exception"] = propertyBag["exception"] = ex;
//						args["controller"] = propertyBag["controller"] = controller;
//						args["method"] = propertyBag["method"] = method;
//
//						rescueExecutor.ProcessSelectedAction(args);
//					}
//				}
//				else
//				{
//					controller._selectedViewName = Path.Combine("rescues", att.ViewName);
//					ProcessView();
//				}
//
//				return true;
//			}
//			catch (Exception exception)
//			{
//				// In this situation, the rescue view could not be found
//				// So we're back to the default error exibition
//
//				if (logger.IsFatalEnabled)
//				{
//					logger.FatalFormat("Failed to process rescue view. View name " +
//									   controller._selectedViewName, exception);
//				}
//			}
//
//			return false;
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

			foreach (RescueDescriptor rescue in rescues)
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

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="action"></param>
		/// <param name="actions"></param>
		/// <returns></returns>
		protected virtual MethodInfo SelectMethod(string action, IDictionary actions,
												  IRequest request, IDictionary actionArgs)
		{
			return null;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="method"></param>
		/// <param name="methodArgs"></param>
		protected virtual void InvokeMethod(MethodInfo method, IRequest request, IDictionary methodArgs)
		{
			method.Invoke(this, new object[0]);
		}

//		/// <summary>
//		/// Creates the default validator runner. 
//		/// </summary>
//		/// <param name="validatorRegistry">The validator registry.</param>
//		/// <returns></returns>
//		/// <remarks>
//		/// You can override this method to create a runner
//		/// with some different configuration
//		/// </remarks>
//		protected virtual ValidatorRunner CreateValidatorRunner(IValidatorRegistry validatorRegistry)
//		{
//			return new ValidatorRunner(validatorRegistry);
//		}

		/// <summary>
		/// Gives a chance to subclasses to format the action name properly
		/// </summary>
		/// <param name="action">Raw action name</param>
		/// <returns>Properly formatted action name</returns>
		internal virtual string TransformActionName(string action)
		{
//		/// <seealso cref="WizardStepPage"/>
			return action;
		}

		/// <summary>
		/// To preserve standard Action semantics when using ASP.NET Views,
		/// the event handlers in the CodeBehind typically call <see cref="Process"/>.
		/// As a result, the <see cref="IsPostBack"/> property must be logically 
		/// cleared to allow the Action to behave as if it was called directly.
		/// </summary>
		private void ResetIsPostback()
		{
			resetIsPostBack = true;
		}
	}
}