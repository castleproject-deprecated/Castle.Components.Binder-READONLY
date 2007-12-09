namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;
	using Container;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IEngineContextFactory
	{
		/// <summary>
		/// Pendent.
		/// </summary>
		/// <returns></returns>
		IEngineContext Create(IMonoRailContainer container, UrlInfo urlInfo, HttpContext context);
	}
}
