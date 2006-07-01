using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Model.Attributes
{
	/// <summary>
	/// Marker attribute that marks a type as one that is used as a configuration
	/// object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class ConfigurationObjectAttribute : Attribute
	{

	}
}
