namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithTwoEndActions : Controller
	{
		public IAsyncResult BeginIndex()
		{
			return null;
		}

		public void EndIndex(IAsyncResult ar)
		{
		}

		public void EndIndex()
		{
		}
	}
}