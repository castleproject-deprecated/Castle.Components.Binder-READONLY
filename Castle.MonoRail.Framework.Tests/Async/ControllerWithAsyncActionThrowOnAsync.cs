namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithAsyncActionThrowOnAsync : Controller
	{
		private readonly Output output;

		public ControllerWithAsyncActionThrowOnAsync()
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
			throw new InvalidOperationException("ba");
		}

		public void EndIndex()
		{
			string s = output.EndInvoke(ControllerContext.AsyncInvocationInformation.AsyncResult);
			RenderText(s);
		}
	}
}