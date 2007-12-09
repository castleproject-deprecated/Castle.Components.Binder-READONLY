namespace Castle.MonoRail.Framework
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using Resources;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ControllerContext : IControllerContext
	{
		private string name;
		private string areaName;
		private string layoutName;
		private string action;
		private string selectedViewName;
		private string viewFolder;
		private IDictionary propertyBag = new HybridDictionary(true);
		private IDictionary helpers = new HybridDictionary(true);
		private IDictionary<string, IDynamicAction> dynamicActions = new Dictionary<string, IDynamicAction>();
		private ResourceDictionary resources = new ResourceDictionary();

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		/// <value></value>
		public IDictionary PropertyBag
		{
			get { return propertyBag; }
			set { propertyBag = value; }
		}

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		public IDictionary Helpers
		{
			get { return helpers; }
			set { helpers = value; }
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		/// <value></value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		/// <value></value>
		public string AreaName
		{
			get { return areaName; }
			set { areaName = value; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		/// <value></value>
		public string LayoutName
		{
			get { return layoutName; }
			set { layoutName = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		/// <value></value>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}

		/// <summary>
		/// Gets or sets the view which will be rendered after this action executes.
		/// </summary>
		/// <value></value>
		public string SelectedViewName
		{
			get { return selectedViewName; }
			set { selectedViewName = value; }
		}

		/// <summary>
		/// Gets the view folder -- (areaname +
		/// controllername) or just controller name -- that this controller
		/// will use by default.
		/// </summary>
		/// <value></value>
		public string ViewFolder
		{
			get { return viewFolder; }
			set { viewFolder = value; }
		}

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <value>The resources.</value>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		public ResourceDictionary Resources
		{
			get { return resources; }
		}

		/// <summary>
		/// Gets the dynamic actions.
		/// </summary>
		/// <value>The dynamic actions.</value>
		public IDictionary<string, IDynamicAction> DynamicActions
		{
			get { return dynamicActions; }
		}
	}
}
