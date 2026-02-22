using System.ComponentModel.DataAnnotations;

namespace Cryptomind.Common.ViewModels.AuthenticationViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(16)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

	}
}
