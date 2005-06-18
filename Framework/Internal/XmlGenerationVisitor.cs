// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Text;

	/// <summary>
	/// 
	/// </summary>
	public class XmlGenerationVisitor : AbstractDepthFirstVisitor
	{
		private StringBuilder xmlBuilder = new StringBuilder();
		private int identLevel = 0;

		public String Xml
		{
			get { return xmlBuilder.ToString(); }
		}

		public override void VisitModel(ActiveRecordModel model)
		{
			xmlBuilder.Length = 0;

			CreateXmlPI();
			StartMappingNode();

			Ident();

			AppendF( "<class {0} {1} {2} {3} {4}>",
				MakeAtt("name", MakeTypeName(model.Type)), 
				MakeAtt("table", model.ActiveRecordAtt.Table),
				WriteIfNonNull("schema", model.ActiveRecordAtt.Schema), 
				WriteIfTrue("proxy", model.ActiveRecordAtt.Proxy),
				WriteIfNonNull("discriminator-value", model.DiscriminatorValue) );

			Ident();

			EnsureOnlyOneKey(model);

			VisitNodes( model.Keys );
			
			CreateDiscriminator();

			VisitNodes( model.Properties );

			VisitNodes( model.SubClasses );

			Dedent();

		    Append( "</class>" );

			Dedent();

			EndMappingNode();
		}

		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			AppendF("<id {0} {1} {2} {3} {4}>",
				MakeAtt("name", model.Property.Name), 
				MakeAtt("column", model.PrimaryKeyAtt.Column),
				WriteIfNonNull("type", model.PrimaryKeyAtt.ColumnType),
				WriteIfNotZero("length", model.PrimaryKeyAtt.Length), 
				WriteIfNonNull("unsaved-value", model.PrimaryKeyAtt.UnsavedValue));

			Ident();

			if (model.PrimaryKeyAtt.Generator != PrimaryKeyType.None)
			{
				AppendF("<generator {0}>", MakeAtt("class", model.PrimaryKeyAtt.Generator.ToString().ToLower()));
				if (model.PrimaryKeyAtt.SequenceName != null)
				{
					Ident();
					AppendF("<param name=\"sequence\">{0}</param>", model.PrimaryKeyAtt.SequenceName);
					Dedent();
				}
				AppendF("</generator>");
			}

			Dedent();
			Append( "</id>" );
		}

		public override void VisitProperty(PropertyModel model)
		{
			AppendF("<property {0} {1} {2} {3} {4} />",
				MakeAtt("name", model.Property.Name), 
				MakeAtt("column", model.PropertyAtt.Column),
				WriteIfNonNull("type", model.PropertyAtt.ColumnType),
				WriteIfNotZero("length", model.PropertyAtt.Length), 
				WriteIfNonNull("unsaved-value", model.PropertyAtt.UnsavedValue),
				WriteIfTrue("not-null", model.PropertyAtt.NotNull), 
				WriteIfTrue("unique", model.PropertyAtt.Unique), 
				WriteIfTrue("insert", model.PropertyAtt.Insert), 
				WriteIfTrue("update", model.PropertyAtt.Update),
				WriteIfNonNull("formula", model.PropertyAtt.Formula));
		}

		private void EnsureOnlyOneKey(ActiveRecordModel model)
		{
			if (model.Keys.Count > 1)
			{
				throw new ActiveRecordException("Composite keys are not supported yet. Type " + model.Type.FullName);
			}
		}

		private void CreateDiscriminator()
		{
			
		}

		#region Xml generations members

		private void CreateXmlPI()
		{
			Append( "<?xml version=\"1.0\" encoding=\"utf-16\"?>" );
		}

		private void StartMappingNode()
		{
			Append( "<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " + 
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">" );
		}

		private void EndMappingNode()
		{
			Append( "</hibernate-mapping>" );
		}

		private String MakeTypeName( Type type )
		{
			return String.Format( "{0}, {1}", type.FullName, type.Assembly.GetName().Name );
		}

		#endregion

		#region String builder helpers

		private String WriteIfNonNull( String attName, String value )
		{
			if (value == null) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfTrue( String attName, bool value )
		{
			if (value == false) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfNotZero( String attName, int value )
		{
			if (value == 0) return String.Empty;
			return MakeAtt(attName, value.ToString());
		}

		private String MakeAtt( String attName, String value )
		{
			return String.Format( "{0}=\"{1}\"", attName, value );
		}

		private String MakeAtt( String attName, bool value )
		{
			return String.Format( "{0}=\"{1}\"", attName, value.ToString().ToLower() );
		}

		private void Append(string xml)
		{
			AppendIdentation();
			xmlBuilder.Append( xml );
			xmlBuilder.Append( "\r\n" );
		}

		private void AppendF(string xml, params object[] args)
		{
			AppendIdentation();
			xmlBuilder.AppendFormat( xml, args );
			xmlBuilder.Append( "\r\n" );
		}

		private void AppendIdentation()
		{
			for(int i=0; i<identLevel; i++)
			{
				xmlBuilder.Append( "  " );
			}
		}

		private void Ident()
		{
			identLevel++;
		}

		private void Dedent()
		{
			identLevel--;
		}

		#endregion
	}
}
