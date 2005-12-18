namespace NVelocity.Dvsl.Directive
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Directive;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;

	/// <summary>
	/// Velocity Directive to handle template registration of
	/// match declarations (like the XSLT match=)
	/// </summary>
	/// <author><a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
	public class MatchDirective : Directive
	{
		public override String Name
		{
			get { return "match"; }
			set { throw new NotSupportedException(); }
		}


		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}


		public override bool Render(InternalContextAdapter context, TextWriter writer, INode node)
		{
			/*
	    *  what is our arg?
	    */
			INode n = node.jjtGetChild(0);

			if (n.Type == ParserTreeConstants.JJTSTRINGLITERAL)
			{
				try
				{
					String element = (String) node.jjtGetChild(0).Value(context);
					TemplateHandler th = (TemplateHandler) rsvc.GetApplicationAttribute("NVelocity.Dvsl.TemplateHandler");

					th.RegisterMatch(element, (SimpleNode) node.jjtGetChild(node.jjtGetNumChildren() - 1));
				}
				catch (Exception ee)
				{
				}
			}

			return true;
		}


	}
}