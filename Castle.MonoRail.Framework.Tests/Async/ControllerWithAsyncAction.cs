namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;
	using System.Threading;

	public class ControllerWithAsyncAction : Controller
	{
		private readonly Output output;

		public ControllerWithAsyncAction()
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
			Thread.Sleep(250);
			return "foo";
		}

		public void EndIndex()
		{
			string s = output.EndInvoke(ControllerContext.AsyncInvocationInformation.AsyncResult);
			RenderText(s);
		}
	}
}