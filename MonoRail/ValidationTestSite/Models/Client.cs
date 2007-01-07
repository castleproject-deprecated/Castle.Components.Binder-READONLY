namespace ValidationTestSite.Models
{
	using Castle.Components.Validator;

	public class Client
	{
		private int id;
		private string name, email;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ValidateNonEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ValidateNonEmpty, ValidateEmail]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}
	}
}
