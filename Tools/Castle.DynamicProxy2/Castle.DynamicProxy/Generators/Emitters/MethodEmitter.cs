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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
#if DOTNET2
	using System.Collections.Generic;
#endif

	[CLSCompliant(false)]
	public class MethodEmitter : IMemberEmitter
	{
		protected MethodBuilder builder;
		protected ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;
		private AbstractTypeEmitter maintype;
#if DOTNET2
		private GenericTypeParameterBuilder[] genericTypeParams;
		private Dictionary<String, GenericTypeParameterBuilder> name2GenericType =
			new Dictionary<string, GenericTypeParameterBuilder>();
#endif
		protected internal MethodEmitter()
		{
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       ReturnReferenceExpression returnRef, params ArgumentReference[] arguments) :
		                       	this(
		                       	maintype, name,
		                       	MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, returnRef,
		                       	arguments)
		{
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name, MethodAttributes attrs)
		{
			this.maintype = maintype;

			builder = maintype.TypeBuilder.DefineMethod(name, attrs);
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       MethodAttributes attrs, ReturnReferenceExpression returnRef,
		                       params ArgumentReference[] arguments) : this(maintype, name, attrs)
		{
			SetParameters(ArgumentsUtil.InitializeAndConvert(arguments));
			SetReturnType(returnRef.Type);
		}

		public void SetReturnType(Type returnType)
		{
			builder.SetReturnType(returnType);
		}
#if DOTNET2
		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}
#endif

		/// <summary>
		/// Inspect the base method for generic definitions
		/// and set the return type and the parameters
		/// accordingly
		/// </summary>
		public void CopyParametersAndReturnTypeFrom(MethodInfo baseMethod, AbstractTypeEmitter parentEmitter)
		{
#if DOTNET2
			GenericUtil.PopulateGenericArguments(parentEmitter, name2GenericType);

			Type[] genericArguments = baseMethod.GetGenericArguments();

			genericTypeParams = GenericUtil.DefineGenericArguments(genericArguments, builder, name2GenericType);
#endif
			// Bind parameter types

			ParameterInfo[] baseMethodParameters = baseMethod.GetParameters();
#if DOTNET2

			SetParameters(GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType));

			// TODO: check if the return type is a generic
			// definition for the method

			SetReturnType(GenericUtil.ExtractCorrectType(baseMethod.ReturnType, name2GenericType));
#else
			SetParameters(GenericUtil.ExtractParameterTypes(baseMethodParameters));
			SetReturnType(baseMethod.ReturnType);
#endif

			DefineParameters(baseMethodParameters);
		}

		public void SetParameters(Type[] paramTypes)
		{
			builder.SetParameters(paramTypes);

			arguments = new ArgumentReference[paramTypes.Length];

			for(int i = 0; i < paramTypes.Length; i++)
			{
				arguments[i] = new ArgumentReference(paramTypes[i]);
			}

			ArgumentsUtil.InitializeArgumentsByPosition(arguments);
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (codebuilder == null)
				{
					codebuilder = new MethodCodeBuilder(
						maintype.BaseType, builder, builder.GetILGenerator());
				}
				return codebuilder;
			}
		}

		public ArgumentReference[] Arguments
		{
			get { return arguments; }
		}

		public /*internal*/ MethodBuilder MethodBuilder
		{
			get { return builder; }
		}

		public Type ReturnType
		{
			get { return builder.ReturnType; }
		}

		public MemberInfo Member
		{
			get { return builder; }
		}

		public virtual void Generate()
		{
			codebuilder.Generate(this, builder.GetILGenerator());
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (CodeBuilder.IsEmpty)
			{
				CodeBuilder.AddStatement(new NopStatement());
				CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public void DefineCustomAttribute(Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);
			if(customAttributeBuilder==null)
				return;
			builder.SetCustomAttribute(customAttributeBuilder);
		}

		private void DefineParameters(ParameterInfo[] info)
		{
			foreach(ParameterInfo parameterInfo in info)
			{
				builder.DefineParameter(parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);
				// builder.DefineGenericParameters()
			}
		}
	}
}
