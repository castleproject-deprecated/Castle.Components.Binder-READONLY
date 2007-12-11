namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IExecutableAction
	{
		/// <summary>
		/// Gets a value indicating whether no filter should run before execute the action.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if they should be skipped; otherwise, <c>false</c>.
		/// </value>
		bool ShouldSkipAllFilters { get; }

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <returns></returns>
		bool ShouldSkipFilter(Type filterType);

		/// <summary>
		/// Gets the layout override.
		/// </summary>
		/// <value>The layout override.</value>
		string LayoutOverride { get;  }

		/// <summary>
		/// Gets the http method that the action requires before being executed.
		/// </summary>
		/// <value>The accessible through verb.</value>
		string AccessibleThroughVerb { get; }

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		void Execute(IEngineContext engineContext, Controller controller, IControllerContext context);
	}
}
