namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithBeginIndexWithoutEndIndex : Controller
	{
		public IAsyncResult BeginIndex()
		{
			return null;
		}
	}
}