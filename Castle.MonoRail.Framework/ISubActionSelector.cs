namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Pendent
	/// </summary>
	public interface ISubActionSelector
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		IExecutableAction Select(IEngineContext engineContext, IController controller, IControllerContext context);
	}
}
