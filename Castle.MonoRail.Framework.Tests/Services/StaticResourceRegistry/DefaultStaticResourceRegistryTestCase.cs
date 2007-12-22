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

namespace Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry
{
	using Castle.Core.Resource;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultStaticResourceRegistryTestCase 
	{
		private DefaultStaticResourceRegistry registry;

		[SetUp]
		public void Init()
		{
			registry = new DefaultStaticResourceRegistry();
		}

		[Test]
		public void Exists_CorrectlyMatchesLocationAndVersion()
		{
			registry.RegisterCustomResource("key", null, null, new StaticContentResource("content"), "text/javascript");

			Assert.IsTrue(registry.Exists("key", null, null));
			Assert.IsTrue(registry.Exists("Key", null, null));
			Assert.IsTrue(registry.Exists("KEY", null, null));

			Assert.IsFalse(registry.Exists("key2", null, null));
			Assert.IsFalse(registry.Exists("key", "neutral", null));
			Assert.IsFalse(registry.Exists("key", "pt-br", null));
			Assert.IsFalse(registry.Exists("key", null, "1"));
			Assert.IsFalse(registry.Exists("key", null, "1.0"));
		}

		[Test]
		public void GetResource_FetchesCorrectResource()
		{
			registry.RegisterCustomResource("key 1", null, null, new StaticContentResource("content 1"), "text/javascript");
			registry.RegisterCustomResource("key 2", null, null, new StaticContentResource("content 2"), "text/javascript");

			string mime;
			Assert.AreEqual("content 1", registry.GetResource("key 1", null, null, out mime));
			Assert.AreEqual("text/javascript", mime);
			Assert.AreEqual("content 2", registry.GetResource("key 2", null, null, out mime));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void RegisterAssemblyResource_FetchesCorrectResource()
		{
			registry.RegisterAssemblyResource("key 1", null, null, "Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry.DummyRes", "jsfunctions", "text/javascript");
			registry.RegisterAssemblyResource("key 2", null, null, "Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry.DummyRes", "jsValidator", "text/javascript");

			string mime;
			Assert.AreEqual("Something 1", registry.GetResource("key 1", null, null, out mime));
			Assert.AreEqual("text/javascript", mime);
			Assert.AreEqual("validators", registry.GetResource("key 2", null, null, out mime));
			Assert.AreEqual("text/javascript", mime);
		}
	}
}
