﻿// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Controllers
{
	using Descriptors;
	using NUnit.Framework;
	using Services;
	using Test;

	[TestFixture]
	public class ControllerTestCase
	{
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			MockResponse response = new MockResponse();
			MockServices services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void ControllerCallsInitialize()
		{
			ControllerWithInitialize controller = new ControllerWithInitialize();

			ControllerContext context = new ControllerContext("controller", "", "action1", new ControllerMetaDescriptor());

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.Initialized);
		}

		[Test]
		public void RendersViewByDefault()
		{
			ControllerAndViews controller = new ControllerAndViews();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "EmptyAction", new ControllerMetaDescriptor());

			controller.Process(engineContext, context);

			Assert.AreEqual("home\\EmptyAction", viewEngStub.TemplateRendered);
		}

		[Test]
		public void ControllerCanOverrideView()
		{
			ControllerAndViews controller = new ControllerAndViews();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "ActionWithViewOverride", new ControllerMetaDescriptor());

			controller.Process(engineContext, context);

			Assert.AreEqual("home\\SomethingElse", viewEngStub.TemplateRendered);
		}

		[Test]
		public void ControllerCanCancelView()
		{
			ControllerAndViews controller = new ControllerAndViews();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "CancelsTheView", new ControllerMetaDescriptor());

			controller.Process(engineContext, context);

			Assert.IsNull(viewEngStub.TemplateRendered);
		}

		#region Controllers

		class ControllerWithInitialize : Controller
		{
			private bool initialized;

			protected override void Initialize()
			{
				initialized = true;
			}

			public bool Initialized
			{
				get { return initialized; }
			}

			public void Action1()
			{
			}
		}

		class ControllerAndViews : Controller
		{
			public void EmptyAction()
			{
			}

			public void ActionWithViewOverride()
			{
				RenderView("SomethingElse");
			}

			public void CancelsTheView()
			{
				CancelView();
			}
		}

		#endregion
	}
}
