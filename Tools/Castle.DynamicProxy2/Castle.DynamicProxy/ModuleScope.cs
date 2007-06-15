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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Resources;
	using System.Threading;
	using Castle.DynamicProxy.Generators;

	/// <summary>
	/// Summary description for ModuleScope.
	/// </summary>
	[CLSCompliant(false)]
	public class ModuleScope
	{
		public static readonly String FILE_NAME = "CastleDynProxy2.dll";
		public static readonly String ASSEMBLY_NAME = "DynamicProxyGenAssembly2";

		/// <summary>
		/// Avoid leaks caused by non disposal of generated types.
		/// </summary>
		private ModuleBuilder moduleBuilderWithStrongName = null;

		private ModuleBuilder moduleBuilder = null;

		/// <summary>
		/// Keep track of generated types
		/// </summary>
		private Hashtable typeCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Used to lock the module builder creation
		/// </summary>
		private object _lockobj = new object();

		private ReaderWriterLock readerWriterLock = new ReaderWriterLock();

		private AssemblyBuilder assemblyBuilder;

		private bool savePhysicalAssembly = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleScope"/> class.
		/// </summary>
		public ModuleScope()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleScope"/> class.
		/// </summary>
		/// <param name="savePhysicalAssembly">if set to <c>true</c> saves the generated module.</param>
		public ModuleScope(bool savePhysicalAssembly)
		{
			this.savePhysicalAssembly = savePhysicalAssembly;
		}

		public ModuleBuilder ObtainDynamicModule()
		{
			return ObtainDynamicModule(false);
		}

		public ModuleBuilder ObtainDynamicModule(bool signStrongName)
		{
			lock(_lockobj)
			{
				if (signStrongName && moduleBuilderWithStrongName == null)
				{
					moduleBuilderWithStrongName = CreateModule(signStrongName);
				}
				else if (!signStrongName && moduleBuilder == null)
				{
					moduleBuilder = CreateModule(signStrongName);
				}
			}

			return signStrongName ? moduleBuilderWithStrongName : moduleBuilder;
		}

		protected internal Type GetFromCache(CacheKey key)
		{
			return (Type) typeCache[key];
		}

		protected internal void RegisterInCache(CacheKey key, Type type)
		{
			typeCache[key] = type;
		}

		public ReaderWriterLock RWLock
		{
			get { return readerWriterLock; }
		}

		/// <summary>
		/// Saves the generated assembly.
		/// </summary>
		public void SaveAssembly()
		{
			if (savePhysicalAssembly)
			{
				if (File.Exists(FILE_NAME))
				{
					File.Delete(FILE_NAME);
				}

				assemblyBuilder.Save(FILE_NAME);
			}
		}

		public Type this[String name]
		{
			get { return typeCache[name] as Type; }
			set
			{
				typeCache[name] = value;
				SaveAssembly();
			}
		}

		private static byte[] GetKeyPair()
		{
			byte[] keyPair;

			using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Castle.DynamicProxy.DynProxy.snk"))
			{
				if (stream == null)
					throw new MissingManifestResourceException(
						"Should have a Castle.DynamicProxy.DynProxy.snk as an embedded resource, so Dynamic Proxy could sign generated assembly");

				int length = (int) stream.Length;
				keyPair = new byte[length];
				stream.Read(keyPair, 0, length);
			}

			return keyPair;
		}

		private ModuleBuilder CreateModule(bool signStrongName)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = ASSEMBLY_NAME;

			if (signStrongName)
			{
				byte[] keyPairStream = GetKeyPair();

				if (keyPairStream != null)
				{
					assemblyName.KeyPair = new StrongNameKeyPair(keyPairStream);
				}
			}

			if (savePhysicalAssembly)
			{
				assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					assemblyName, AssemblyBuilderAccess.RunAndSave);

				moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, FILE_NAME, true);

				return moduleBuilder;
			}
			else
			{
				assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					assemblyName,
					AssemblyBuilderAccess.Run);

				return assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);
			}
		}
	}
}