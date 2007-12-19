// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Xml;
	using Castle.Core.Configuration;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IMonoRailConfiguration
	{
		/// <summary>
		/// Gets the SMTP config.
		/// </summary>
		/// <value>The SMTP config.</value>
		SmtpConfig SmtpConfig { get; }

		/// <summary>
		/// Gets the view engine config.
		/// </summary>
		/// <value>The view engine config.</value>
		ViewEngineConfig ViewEngineConfig { get; }

		/// <summary>
		/// Gets the controllers config.
		/// </summary>
		/// <value>The controllers config.</value>
		ControllersConfig ControllersConfig { get; }

		/// <summary>
		/// Gets the view components config.
		/// </summary>
		/// <value>The view components config.</value>
		ViewComponentsConfig ViewComponentsConfig { get; }

		/// <summary>
		/// Gets the routing rules.
		/// </summary>
		/// <value>The routing rules.</value>
		RoutingRuleCollection RoutingRules { get; }

		/// <summary>
		/// Gets the extension entries.
		/// </summary>
		/// <value>The extension entries.</value>
		ExtensionEntryCollection ExtensionEntries { get; }

		/// <summary>
		/// Gets the custom filter factory.
		/// </summary>
		/// <value>The custom filter factory.</value>
		Type CustomFilterFactory { get; }

		/// <summary>
		/// Gets the scaffold config.
		/// </summary>
		/// <value>The scaffold config.</value>
		ScaffoldConfig ScaffoldConfig { get; }

		/// <summary>
		/// Gets the url config.
		/// </summary>
		/// <value>The url config.</value>
		UrlConfig UrlConfig { get; }

		/// <summary>
		/// Gets a value indicating whether MR should check for client connection.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should check client is connected; otherwise, <c>false</c>.
		/// </value>
		bool CheckClientIsConnected { get; }

		/// <summary>
		/// Gets a value indicating whether to use windsor integration.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should use windsor integration; otherwise, <c>false</c>.
		/// </value>
		bool UseWindsorIntegration { get; }

		/// <summary>
		/// Gets a value indicating whether match host name and path should be used on 
		/// MonoRail routing.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should match host name and path; otherwise, <c>false</c>.
		/// </value>
		bool MatchHostNameAndPath { get; }

		/// <summary>
		/// Gets a value indicating whether routing should exclude app path.
		/// </summary>
		/// <value><c>true</c> if exclude app path; otherwise, <c>false</c>.</value>
		bool ExcludeAppPath { get; }

		/// <summary>
		/// Gets the default urls.
		/// </summary>
		/// <value>The default urls.</value>
		DefaultUrlCollection DefaultUrls { get; }

		/// <summary>
		/// Gets the services config.
		/// </summary>
		/// <value>The services config.</value>
		IConfiguration ServicesConfig { get; }

		/// <summary>
		/// Gets the configuration section.
		/// </summary>
		/// <value>The configuration section.</value>
		XmlNode ConfigurationSection { get; }
	}
}
