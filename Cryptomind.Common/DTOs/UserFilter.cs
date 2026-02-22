namespace Cryptomind.Common.DTOs
{
	public class UserFilter
	{
		public bool? IsBanned { get; set; }
		public bool? IsDeactivated { get; set; }
        public string? Username { get; set; }
    }
}
