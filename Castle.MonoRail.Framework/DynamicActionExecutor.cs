namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DynamicActionExecutor : IExecutableAction
	{
		private readonly IDynamicAction action;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicActionExecutor"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public DynamicActionExecutor(IDynamicAction action)
		{
			this.action = action;
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
		/// Gets the layout override.
		/// </summary>
		/// <value>The layout override.</value>
		public string LayoutOverride
		{
			get { return null; }
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
		public void Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			action.Execute(engineContext, controller, context);
		}
	}
}
