namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Reflection;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionMethodExecutor : IExecutableAction
	{
		/// <summary>
		/// Pendent
		/// </summary>
		protected readonly MethodInfo actionMethod;
		/// <summary>
		/// Pendent
		/// </summary>
		protected readonly ActionMetaDescriptor metaDescriptor;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionMethodExecutor"/> class.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		public ActionMethodExecutor(MethodInfo actionMethod, ActionMetaDescriptor metaDescriptor)
		{
			this.actionMethod = actionMethod;
			this.metaDescriptor = metaDescriptor;

//				string layoutName = null;
//
//				// Overrides the current layout, if the action specifies one
//				if (actionDesc.Layout != null)
//				{
//					layoutName = actionDesc.Layout.LayoutName;
//				}

//				if (actionDesc.AccessibleThrough != null)
//				{
//					if (!actionDesc.AccessibleThrough.ForHttpMethod(engineContext.Request.HttpMethod))
//					{
//						throw new ControllerException(string.Format("Access to the action [{0}] " +
//																	"on controller [{1}] is not allowed by the http verb [{2}].",
//																	actionName, Name, engineContext.Request.HttpMethod));
//					}
//				}
		}

		/// <summary>
		/// Gets a value indicating whether no filter should run before execute the action.
		/// </summary>
		/// <value><c>true</c> if they should be skipped; otherwise, <c>false</c>.</value>
		public bool ShouldSkipAllFilters
		{
			get { return false; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <returns></returns>
		public bool ShouldSkipFilter(Type filterType)
		{
			return false;
		}

		/// <summary>
		/// Gets the layout override.
		/// </summary>
		/// <value>The layout override.</value>
		public string LayoutOverride
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the http method that the action requires before being executed.
		/// </summary>
		/// <value>The accessible through verb.</value>
		public string AccessibleThroughVerb
		{
			get { return null; }
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public virtual void Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			actionMethod.Invoke(controller, null);
		}
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionMethodExecutorCompatible : ActionMethodExecutor
	{
		private readonly InvokeOnController invoke;

		/// <summary>
		/// Pendent
		/// </summary>
		public delegate void InvokeOnController(MethodInfo method, IRequest request, IDictionary methodArgs);

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionMethodExecutorCompatible"/> class.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		/// <param name="invoke">The invoke.</param>
		public ActionMethodExecutorCompatible(MethodInfo actionMethod, ActionMetaDescriptor metaDescriptor, InvokeOnController invoke) : base(actionMethod, metaDescriptor)
		{
			this.invoke = invoke;
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public override void Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			invoke(actionMethod, engineContext.Request, null);
		}
	}
}
