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

		public bool ShouldSkipAllFilters
		{
			get { return false; }
		}

		public string LayoutOverride
		{
			get { return null; }
		}

		public bool ShouldSkipFilter(Type filterType)
		{
			return false;
		}

		public string AccessibleThroughVerb
		{
			get { return null; }
		}

		public void Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			action.Execute(engineContext, controller, context);
		}
	}
}
