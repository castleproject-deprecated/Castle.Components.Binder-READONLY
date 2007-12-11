namespace Castle.MonoRail.Framework.Tests
{
	using NUnit.Framework;
	using Services;

	[TestFixture]
	public class DefaultActionSelectorTestCase
	{
		private DefaultActionSelector selector = new DefaultActionSelector();

		[Test]
		public void a()
		{
			selector.Select(en, controller, context);
		}
	}
}
