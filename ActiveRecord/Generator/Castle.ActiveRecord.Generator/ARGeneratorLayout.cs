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

namespace Castle.ActiveRecord.Generator
{
	using System;

	using Castle.ActiveRecord.Generator.Actions;
	using Castle.ActiveRecord.Generator.Parts;


	public class ARGeneratorLayout : IApplicationLayout
	{
		private Model _model;

		public ARGeneratorLayout(Model model)
		{
			_model = model;
		}

		#region IApplicationLayout Members

		public void Install(IWorkspace workspace)
		{
			// Register Actions
			
			FileActionGroup group1 = new FileActionGroup();
			group1.Init(_model);
			group1.Install(workspace);

			// Add parts

			ActiveRecordGraphView arGraph = new ActiveRecordGraphView(_model);
			arGraph.Show(workspace.MainDockManager);

			OutputView outView = new OutputView(_model);
			outView.Show(workspace.MainDockManager);

			ProjectExplorer projExplorer = new ProjectExplorer(_model);
			projExplorer.Show(workspace.MainDockManager);

			AvailableShapes avaShapes = new AvailableShapes(_model);
			avaShapes.Show(workspace.MainDockManager);
		}

		public void Persist()
		{
		}

		#endregion
	}
}
