namespace Castle.MonoRail.Framework.Tests.Providers
{
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultLayoutDescriptorProviderTestCase
	{
		private DefaultLayoutDescriptorProvider provider = new DefaultLayoutDescriptorProvider();

		[Test]
		public void CanCollectLayoutFromClass()
		{
			LayoutDescriptor desc = provider.CollectLayout(typeof(LayoutOnController));
	
			Assert.IsNotNull(desc);
			Assert.AreEqual("default", desc.LayoutName);
		}

		[Test]
		public void CanCollectLayoutFromMethod()
		{
			LayoutDescriptor desc = provider.CollectLayout(typeof(LayoutOnActionController).GetMethod("Action"));

			Assert.IsNotNull(desc);
			Assert.AreEqual("action", desc.LayoutName);
		}

		#region Controllers

		[Layout("default")]
		public class LayoutOnController : Controller
		{
		}

		public class LayoutOnActionController : Controller
		{
			[Layout("action")]
			public void Action()
			{
			}
		}

		#endregion
	}
}
