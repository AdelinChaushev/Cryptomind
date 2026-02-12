using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UploadedCiphers = new List<Cipher>();
            HintsRequested = new List<HintRequest>();
            Notifications = new List<Notification>();
        }
        public int Score { get; set; }
        public int SolvedCount { get; set; }
        public int AttemptedCiphers { get; set; }
		public bool isBanned { get; set; }
		public string? BanReason { get; set; }
        public int LeaderBoardPlace { get; set; }
		public DateTime RegisteredAt { get; set; }
		public DateTime? BannedAt { get; set; }
		public double SuccessRate => CalculateSuccessRate();
        public ICollection<Cipher> UploadedCiphers { get; set; }
        public ICollection<UserSolution> SolvedCiphers { get; set; }
        public ICollection<HintRequest> HintsRequested { get; set; }
        public ICollection<AnswerSuggestion> SuggestedAnswers { get; set; }
        public ICollection <UserBadge> Badges { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        private double CalculateSuccessRate()
        {
            if (SolvedCount == 0 || AttemptedCiphers == 0) return 0;

            return (double)(SolvedCount / AttemptedCiphers) * 100;
        }
	}
}
