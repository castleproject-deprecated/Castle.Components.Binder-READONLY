// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Anakia
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Xml;
	using Commons.Collections;
	using NAnt.Core;
	using NAnt.Core.Attributes;
	using NVelocity;
	using NVelocity.App;

	[TaskName("doctransform")]
	public class NDocTransformTask : Task
	{
		private VelocityEngine velocity;
		private Template template;
		private DirectoryInfo targetDir;
		private string nodcxmlfile;
		private string templateFile;
		private string inheritsFrom;
		private XmlDocument projectDom;

		[TaskAttribute("templatefile")]
		public String TemplateFile
		{
			get { return templateFile; }
			set { templateFile = value; }
		}

		[TaskAttribute("ndocxmlfile")]
		public String NDocXmlFile
		{
			get { return nodcxmlfile; }
			set { nodcxmlfile = value; }
		}
		
		[TaskAttribute("targetdir")]
		public DirectoryInfo TargetDir
		{
			get { return targetDir; }
			set { targetDir = value; }
		}
		
		[TaskAttribute("inheritsfrom")]
		public String InheritsFrom
		{
			get { return inheritsFrom; }
			set { inheritsFrom = value; }
		}
		
		#region overrides

		protected override void InitializeTask(XmlNode taskNode)
		{
			base.InitializeTask(taskNode);

			// Initializes NVelocity

			velocity = new VelocityEngine();

			ExtendedProperties props = new ExtendedProperties();
			velocity.Init(props);

			template = velocity.GetTemplate(templateFile);

			// TODO: validate all arguments are present
		}

		protected override void ExecuteTask()
		{
			projectDom = new XmlDocument();
			projectDom.Load(NDocXmlFile);

			try
			{
				//T:System.Attribute
				XmlNode baseNode = projectDom.SelectSingleNode("/hierarchyType/@id='T:System.Attribute'"); 
				
				foreach(XmlNode child in baseNode)
				{
					
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				Console.Read();
			}
		}

		#endregion
	}
}
