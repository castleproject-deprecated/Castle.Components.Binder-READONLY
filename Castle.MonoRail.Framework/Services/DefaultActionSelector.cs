namespace Castle.MonoRail.Framework.Services
{
	using System.Collections.Generic;
	using System.Reflection;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultActionSelector : IActionSelector
	{
		private List<ISubActionSelector> subSelectors = new List<ISubActionSelector>();

		/// <summary>
		/// Selects the an action.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public IExecutableAction Select(IEngineContext engineContext, IController controller, IControllerContext context)
		{
			string actionName = context.Action;

			// Look for the target method
			MethodInfo actionMethod = SelectActionMethod(controller, context, context.Action);

			if (actionMethod == null)
			{
				// If we couldn't find a method for this action, look for a dynamic action
				IDynamicAction dynAction = null;

				if (context.DynamicActions.ContainsKey(actionName))
				{
					dynAction = context.DynamicActions[actionName];
				}

				if (dynAction != null)
				{
					return new DynamicActionExecutor(dynAction);
				}
			}
			else
			{
				ActionMetaDescriptor actionDesc = context.ControllerDescriptor.GetAction(actionMethod);

				return new ActionMethodExecutor(actionMethod, actionDesc);
			}

			IExecutableAction executableAction = RunSubSelectors(engineContext, controller, context);

			if (executableAction != null)
			{
				return executableAction;
			}

			executableAction = ResolveDefaultMethod(context.ControllerDescriptor, controller, context);

			if (executableAction == null)
			{
				throw new ControllerException(string.Format("Unable to find action '{0}' on controller '{1}'.", actionName, Name));
			}

			return executableAction;
		}

		/// <summary>
		/// Registers the specified sub selector.
		/// </summary>
		/// <param name="subSelector">The sub selector.</param>
		public void Register(ISubActionSelector subSelector)
		{
			subSelectors.Add(subSelector);
		}

		/// <summary>
		/// Unregisters the specified sub selector.
		/// </summary>
		/// <param name="subSelector">The sub selector.</param>
		public void Unregister(ISubActionSelector subSelector)
		{
			subSelectors.Remove(subSelector);
		}

		protected virtual MethodInfo SelectActionMethod(IController controller, IControllerContext context, string name)
		{
			return context.ControllerDescriptor.Actions[name] as MethodInfo;
		}

		protected virtual IExecutableAction RunSubSelectors(IEngineContext engineContext, IController controller, IControllerContext context)
		{
			foreach(ISubActionSelector selector in subSelectors)
			{
				IExecutableAction action = selector.Select(engineContext, controller, context);

				if (action != null)
				{
					return action;
				}
			}

			return null;
		}

		/// <summary>
		/// The following lines were added to handle _default processing
		/// if present look for and load _default action method
		/// <seealso cref="DefaultActionAttribute"/>
		/// </summary>
		private IExecutableAction ResolveDefaultMethod(ControllerMetaDescriptor controllerDesc, IController controller, IControllerContext context)
		{
			if (controllerDesc.DefaultAction != null)
			{
				MethodInfo method = SelectActionMethod(
					controller, context, 
					controllerDesc.DefaultAction.DefaultAction);

				if (method != null)
				{
					ActionMetaDescriptor actionDesc = context.ControllerDescriptor.GetAction(method);

					return new ActionMethodExecutor(method, actionDesc);
				}
			}

			return null;
		}
	}
}
