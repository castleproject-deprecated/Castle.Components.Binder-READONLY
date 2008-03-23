namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithAsyncActionThrowOnBegin : Controller
	{
		private readonly Output output;

		public ControllerWithAsyncActionThrowOnBegin()
		{
			output = ((Output) LongOp);
		}

		public delegate string Output();

		public IAsyncResult BeginIndex()
		{
			throw new InvalidOperationException("ba");
		}

		private string LongOp()
		{
			return "foo";
		}

		public void EndIndex()
		{
			string s = output.EndInvoke(ControllerContext.AsyncInvocationInformation.AsyncResult);
			RenderText(s);
		}
	}
}