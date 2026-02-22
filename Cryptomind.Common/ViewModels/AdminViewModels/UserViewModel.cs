namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Role { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public bool IsAdmin { get; set; }
		public int PendingCiphers { get; set; }
	}
}
