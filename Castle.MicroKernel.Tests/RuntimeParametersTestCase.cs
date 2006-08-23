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

namespace Castle.MicroKernel.Tests
{
	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.MicroKernel.Handlers;

	using Castle.MicroKernel.Tests.RuntimeParameters;

	/// <summary>
	/// Summary description for DependencyResolvers.
	/// </summary>
	[TestFixture]
	public class RuntimeParametersTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void ResolveUsingParameters()
		{
			kernel.AddComponent("compa", typeof(CompA));
			kernel.AddComponent("compb", typeof(CompB));

			CompB compb = null;
			compb = kernel.Resolve(typeof(CompB), new object[] { new CompC(12), "ernst" }) as CompB;

			Assert.IsNotNull(compb, "Component B should have been resolved");

			Assert.IsNotNull(compb.Compc, "CompC property should not be null");
			Assert.IsTrue(compb.MyArgument != string.Empty, "MyArgument property should not be empty");

			Assert.IsTrue(typeof(CompC).IsAssignableFrom(compb.Compc.GetType()), "CompC property should be null assignable from CompC");
			Assert.IsTrue("ernst".Equals(compb.MyArgument),string.Format( "The MyArgument property of compb should be equal to ernst, found {0}", compb.MyArgument));
		}
	}
}
