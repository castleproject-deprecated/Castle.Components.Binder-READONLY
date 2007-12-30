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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Pendent
	/// </summary>
	[DebuggerDisplay("PatternRoute {pattern}")]
	public class PatternRoute : IRoutingRule
	{
		private readonly string pattern;
		private readonly List<DefaultNode> nodes = new List<DefaultNode>();

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternRoute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		public PatternRoute(string pattern)
		{
			this.pattern = pattern;
			CreatePatternNodes();
		}

		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		public string RouteName
		{
			get { return null; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="hostname">The hostname.</param>
		/// <param name="virtualPath">The virtual path.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string hostname, string virtualPath, IDictionary parameters)
		{
			StringBuilder text = new StringBuilder(virtualPath);

			foreach(DefaultNode node in nodes)
			{
				if (text.Length == 0 || text[text.Length - 1] != '/')
				{
					text.Append('/');
				}

				if (node.name == null)
				{
					text.Append(node.start);
				}
				else
				{
					object value = parameters[node.name];

					if (value == null)
					{
						if (!node.optional)
						{
							return null;
						}
						else
						{
							break;
						}
					}
					else
					{
						text.Append(value.ToString());
					}
				}
			}

			return text.ToString();
		}

		/// <summary>
		/// Determines if the specified URL matches the
		/// routing rule.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The context</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public bool Matches(string url, IRouteContext context, RouteMatch match)
		{
			string[] parts = url.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
			int index = 0;

			foreach(DefaultNode node in nodes)
			{
				string part = index < parts.Length ? parts[index] : null;

				if (!node.Matches(part, match))
				{
					return false;
				}

				index++;
			}

			return true;
		}

		private void CreatePatternNodes()
		{
			string[] parts = pattern.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

			foreach(string part in parts)
			{
				if (part.Contains("["))
				{
					nodes.Add(CreateNamedOptionalNode(part));
				}
				else
				{
					nodes.Add(CreateRequiredNode(part));
				}
			}
		}

		private DefaultNode CreateNamedOptionalNode(string part)
		{
			return new DefaultNode(part, true);
		}

		private DefaultNode CreateRequiredNode(string part)
		{
			return new DefaultNode(part, false);
		}

		#region DefaultNode

		[DebuggerDisplay("Node {name} Opt: {optional} default: {defaultVal} Regular exp: {exp}")]
		private class DefaultNode
		{
			public readonly string name, start, end;
			public readonly bool optional;
			private string defaultVal;
			private bool acceptsIntOnly;
			private string[] acceptedTokens;
			private Regex exp;

			public DefaultNode(string part, bool optional)
			{
				this.optional = optional;
				int indexStart = part.IndexOfAny(new char[] {'<', '['});
				int indexEndStart = -1;

				if (indexStart != -1)
				{
					indexEndStart = part.IndexOfAny(new char[] {'>', ']'}, indexStart);
					name = part.Substring(indexStart + 1, indexEndStart - indexStart - 1);
				}

				if (indexStart != -1)
				{
					start = part.Substring(0, indexStart);
				}
				else
				{
					start = part;
				}

				end = "";

				if (indexEndStart != -1)
				{
					end = part.Substring(indexEndStart + 1);
				}

				ReBuildRegularExpression();
			}

			private void ReBuildRegularExpression()
			{
				RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline;

				if (name != null)
				{
					exp = new Regex("^" + CharClass(start) + "(" + GetExpression() + ")" + CharClass(end) + "$", options);
				}
				else
				{
					exp = new Regex("^(" + CharClass(start) + ")$");
				}
			}

			private string GetExpression()
			{
				if (acceptsIntOnly)
				{
					return "[0-9]+";
				}
				else if (acceptedTokens != null && acceptedTokens.Length != 0)
				{
					StringBuilder text = new StringBuilder();

					foreach(string token in acceptedTokens)
					{
						if (text.Length != 0)
						{
							text.Append("|");
						}
						text.Append("(");
						text.Append(CharClass(token));
						text.Append(")");
					}

					return text.ToString();
				}
				else
				{
					return "[a-zA-Z,0-9]+";
				}
			}

			public bool Matches(string part, RouteMatch match)
			{
				if (part == null)
				{
					if (optional)
					{
						if (name != null)
						{
							match.AddNamed(name, defaultVal);
						}
						return true;
					}
					else
					{
						return false;
					}
				}

				Match regExpMatch = exp.Match(part);

				if (regExpMatch.Success)
				{
					if (name != null)
					{
						match.AddNamed(name, part);
					}

					return true;
				}

				return false;
			}

			public void AcceptsAnyOf(string[] names)
			{
				acceptedTokens = names;
				ReBuildRegularExpression();
			}

			public string DefaultVal
			{
				set { defaultVal = value; }
			}

			public bool AcceptsIntOnly
			{
				set
				{
					acceptsIntOnly = value;
					ReBuildRegularExpression();
				}
			}
		}

		#endregion

		/// <summary>
		/// Configures the default for the named pattern part.
		/// </summary>
		/// <param name="namedPatternPart">The named pattern part.</param>
		/// <returns></returns>
		public DefaultConfigurer DefaultFor(string namedPatternPart)
		{
			return new DefaultConfigurer(this, namedPatternPart);
		}

		/// <summary>
		/// Configures restrictions for the named pattern part.
		/// </summary>
		/// <param name="namedPatternPart">The named pattern part.</param>
		/// <returns></returns>
		public RestrictionConfigurer Restrict(string namedPatternPart)
		{
			return new RestrictionConfigurer(this, namedPatternPart);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class RestrictionConfigurer
		{
			private readonly PatternRoute route;
			private readonly DefaultNode targetNode;

			/// <summary>
			/// Initializes a new instance of the <see cref="RestrictionConfigurer"/> class.
			/// </summary>
			/// <param name="route">The route.</param>
			/// <param name="namedPatternPart">The named pattern part.</param>
			public RestrictionConfigurer(PatternRoute route, string namedPatternPart)
			{
				this.route = route;
				targetNode = route.GetNamedNode(namedPatternPart);
			}

			/// <summary>
			/// Anies the of.
			/// </summary>
			/// <param name="validNames">The valid names.</param>
			/// <returns></returns>
			public PatternRoute AnyOf(params string[] validNames)
			{
				targetNode.AcceptsAnyOf(validNames);
				return route;
			}

			/// <summary>
			/// Gets the valid integer.
			/// </summary>
			/// <value>The valid integer.</value>
			public PatternRoute ValidInteger
			{
				get
				{
					targetNode.AcceptsIntOnly = true;
					return route;
				}
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class DefaultConfigurer
		{
			private readonly PatternRoute route;
			private readonly DefaultNode targetNode;

			/// <summary>
			/// Initializes a new instance of the <see cref="DefaultConfigurer"/> class.
			/// </summary>
			/// <param name="patternRoute">The pattern route.</param>
			/// <param name="namedPatternPart">The named pattern part.</param>
			public DefaultConfigurer(PatternRoute patternRoute, string namedPatternPart)
			{
				route = patternRoute;
				targetNode = route.GetNamedNode(namedPatternPart);
			}

			/// <summary>
			/// Sets the default value for this named pattern part.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns></returns>
			public PatternRoute Is(string value)
			{
				targetNode.DefaultVal = value;
				return route;
			}

			/// <summary>
			/// Sets the default value as empty for this named pattern part.
			/// </summary>
			/// <value>The is empty.</value>
			public PatternRoute IsEmpty
			{
				get
				{
					targetNode.DefaultVal = string.Empty;
					return route;
				}
			}
		}

		// See http://weblogs.asp.net/justin_rogers/archive/2004/03/20/93379.aspx
		private static string CharClass(string content)
		{
			if (content == String.Empty)
			{
				return string.Empty;
			}

			StringBuilder builder = new StringBuilder();

			foreach(char c in content)
			{
				if (char.IsLetter(c))
				{
					builder.AppendFormat("[{0}{1}]", char.ToLower(c), char.ToUpper(c));
				}
				else
				{
					builder.Append(c);
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets the named node.
		/// </summary>
		/// <param name="part">The part.</param>
		/// <returns></returns>
		private DefaultNode GetNamedNode(string part)
		{
			DefaultNode found = nodes.Find(delegate(DefaultNode node) { return node.name == part; });

			if (found == null)
			{
				throw new ArgumentException("Could not find pattern node for name " + part);
			}

			return found;
		}
	}
}