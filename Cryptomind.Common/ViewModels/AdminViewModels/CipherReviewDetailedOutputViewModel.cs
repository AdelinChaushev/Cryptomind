

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class CipherDetailedReviewOutputViewModel : CipherReviewOutputViewModel
	{
		public int Points { get; set; }
		public string CreatorUserName { get; set; }

        public string DecryptedText { get; set; }
        public string CipherText { get; set; }
		public bool AllowFullSolution { get; set; }
		public bool AllowType { get; set; }
		public bool AllowHint { get; set; }
        public string Status { get; set; }

        public string ChallengeTypeDisplay { get; set; }
        public string ImageBase64 { get; set; }
	}
}
