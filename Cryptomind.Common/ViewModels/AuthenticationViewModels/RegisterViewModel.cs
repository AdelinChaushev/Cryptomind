using System.ComponentModel.DataAnnotations;

namespace Cryptomind.Common.ViewModels.AuthenticationViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[MaxLength(16)]

		public string Username { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[MaxLength(16)]
		[MinLength(8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[MaxLength(16)]
		[MinLength(8)]
		[Compare(nameof(Password))]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}
}
