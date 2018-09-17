namespace CodeHub.Models
{
	public class Account
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public string AvatarUrl { get; set; }
		public bool IsLoggedIn { get; set; }
		public bool IsActive { get; set; }
	}
}
