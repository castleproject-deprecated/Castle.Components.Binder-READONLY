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

namespace Castle.Facilities.TypedFactory.Tests.Factories
{
	using System;

	using Castle.Facilities.TypedFactory.Tests.Components;

	public interface IProtocolHandlerFactory1
	{
		IProtocolHandler Create();

		void Release(IProtocolHandler handler);
	}

	public interface IProtocolHandlerFactory2
	{
		IProtocolHandler Create(String key);

		void Release(IProtocolHandler handler);
	}
}