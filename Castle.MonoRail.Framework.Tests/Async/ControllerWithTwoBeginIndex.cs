namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithTwoBeginIndex : Controller
	{
		public IAsyncResult BeginIndex()
		{
			return null;
		}

		public IAsyncResult BeginIndex(string x)
		{
			return null;
		}
	}
}