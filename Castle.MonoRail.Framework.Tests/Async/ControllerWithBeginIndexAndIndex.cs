namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithBeginIndexAndIndex : Controller
	{
		public IAsyncResult BeginIndex()
		{
			return null;
		}

		public void Index()
		{
		}
	}
}