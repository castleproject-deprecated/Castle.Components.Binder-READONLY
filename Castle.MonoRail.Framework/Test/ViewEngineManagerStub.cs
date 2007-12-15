namespace Castle.MonoRail.Framework.Test
{
	using System.IO;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ViewEngineManagerStub : IViewEngineManager
	{
		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// <para>
		/// Please note that no layout is applied
		/// </para>
		/// </summary>
		public void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		                    IControllerContext controllerContext)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Processes a partial view using the partialName
		/// to obtain the correct template and writes the
		/// results to the System.TextWriter.
		/// </summary>
		public void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller,
		                           IControllerContext controllerContext)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
		                                     IControllerContext controllerContext)
		{
			throw new System.NotImplementedException();
		}
	}
}
