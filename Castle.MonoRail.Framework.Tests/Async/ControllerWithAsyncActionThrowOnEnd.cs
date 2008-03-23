namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithAsyncActionThrowOnEnd : Controller
	{
		private readonly Output output;

		public ControllerWithAsyncActionThrowOnEnd()
		{
			output = ((Output) LongOp);
		}

		public delegate string Output();

		public IAsyncResult BeginIndex()
		{
			return output.BeginInvoke(delegate { }, null);
		}

		private string LongOp()
		{
			return "foo";
		}

		public void EndIndex()
		{
			throw new InvalidOperationException("b");
		}
	}
}