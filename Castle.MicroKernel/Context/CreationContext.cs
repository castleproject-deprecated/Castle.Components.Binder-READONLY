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

using System.Collections;
using System.Text;
using Castle.MicroKernel.Exceptions;
using Castle.Model;

namespace Castle.MicroKernel
{
	using System;

	[Serializable]
	public sealed class CreationContext
	{
        private readonly ArrayList dependencies;
	    public readonly static CreationContext Empty = new CreationContext(new DependencyModel[0]);

		#if DOTNET2
		private readonly Type[] arguments;
		#endif

        public CreationContext(ICollection dependencies)
        {
            this.dependencies = new ArrayList(dependencies);
        }

        public void AddDependncy(DependencyModel dependencyModel)
        {
            if (dependencies.Contains(dependencyModel))
            {
                StringBuilder sb = new StringBuilder("A cycle was detected when creating the service.");
                foreach (DependencyModel dependency in dependencies)
                {
                    sb.AppendLine().AppendFormat("\t- {0}", dependency);
                }
                sb.AppendLine().
                    AppendFormat("The dependecy that caused the cycle is: {0}", dependencyModel).
                    AppendLine();
                throw new CircularDependecyException(sb.ToString());
            }
            dependencies.Add(dependencyModel);
        }

	    public ICollection Dependencies
	    {
	        get { return dependencies; }
	    }

#if DOTNET2
		
		public CreationContext(ICollection dependencies, Type target)
		{
            this.dependencies = new ArrayList(dependencies);
		    arguments = ExtractGenericArguments(target);
		}

		public Type[] GenericArguments
		{
			get { return arguments; }
		}

		private static Type[] ExtractGenericArguments(Type target)
		{
			return target.GetGenericArguments();
		}
	    
		#endif
	    
	}
}
