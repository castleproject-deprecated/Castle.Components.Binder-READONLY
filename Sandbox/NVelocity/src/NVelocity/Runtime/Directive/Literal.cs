namespace NVelocity.Runtime.Directive
{
	/*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2000-2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Parser.Node;

	/// <summary> A very simple directive that leverages the Node.literal()
	/// to grab the literal rendition of a node. We basically
	/// grab the literal value on init(), then repeatedly use
	/// that during render().
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <version> $Id: Literal.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class Literal : Directive
	{
		public override String Name
		{
			get { return "literal"; }
			set { throw new NotSupportedException(); }
		}

		public override int Type
		{
			get { return DirectiveConstants_Fields.BLOCK; }

		}

		internal String literalText;

		/// <summary> Return name of this directive.
		/// </summary>
		/// <summary> Return type of this directive.
		/// </summary>
		/// <summary> Store the literal rendition of a node using
		/// the Node.literal().
		/// </summary>
		public override void init(RuntimeServices rs, InternalContextAdapter context, INode node)
		{
			base.init(rs, context, node);

			literalText = node.jjtGetChild(0).literal();
		}

		/// <summary> Throw the literal rendition of the block between
		/// #literal()/#end into the writer.
		/// </summary>
		public override bool render(InternalContextAdapter context, TextWriter writer, INode node)
		{
			writer.Write(literalText);
			return true;
		}
	}
}