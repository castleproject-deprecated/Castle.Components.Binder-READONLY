namespace TempWeb
{
	using Castle.MonoRail.WindsorExtension;
	using Castle.Windsor;

	public class Global : System.Web.HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		public void Application_OnStart()
		{
			// container = new WindsorContainer(new XmlInterpreter(new ConfigResource()));

			container = new WindsorContainer();
			container.AddFacility("rails", new RailsFacility());
		}

		public void Application_OnEnd()
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}