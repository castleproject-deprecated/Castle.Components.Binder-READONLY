using System.Collections;
using Castle.CastleOnRails.Framework.Helpers;
// ${Copyrigth}

namespace TestSite.Controllers
{
	using System;

	using Castle.CastleOnRails.Framework;
	
	[Helper( typeof(AjaxHelper) )]
	public class AjaxController : SmartDispatcherController
	{
		public AjaxController()
		{
		}

		public void Index()
		{
			PropertyBag["users"] = GetList();
		}

		public void InferAddress()
		{
			RenderText("<b>pereira leite st, 44<b>");
		}

		public void AccountFormValidate(String name, String addressf)
		{
			String message = "";

			if (name == null || name.Length == 0)
			{
				message = "<b>Please, dont forget to enter the name<b>";
			}
			if (addressf == null || addressf.Length == 0)
			{
				message += "<b>Please, dont forget to enter the address<b>";
			}

			if (message == "")
			{
				message = "Seems that you know how to fill a form! :-)";
			}

			RenderText(message);
		}

		public void AddUserWithAjax(String userNameField, String email)
		{
			GetList().Add( new User(userNameField, email) );

			Index();

			RenderView("Index");
		}

		private IList GetList()
		{
			IList list = Context.Session["list"] as IList;
			
			if (list == null)
			{
				list = new ArrayList();

				list.Add( new User("somefakeuser", "fakeemail@server.net") );
				list.Add( new User("someotherfakeuser", "otheremail@server.net") );

				Context.Session["list"] = list;
			}
			
			return list;
		}
	}

	public class User
	{
		private String name;
		private String email;

		public User(string name, string email)
		{
			this.name = name;
			this.email = email;
		}

		public string Name
		{
			get { return name; }
		}

		public string Email
		{
			get { return email; }
		}
	}
}
