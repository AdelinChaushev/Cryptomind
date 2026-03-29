using System.ComponentModel.DataAnnotations;

namespace Cryptomind.Common.DTOs
{
	public class DailyChallengeSubmitDTO
	{
		[Required]
		public string Answer { get; set; }
	}
}
