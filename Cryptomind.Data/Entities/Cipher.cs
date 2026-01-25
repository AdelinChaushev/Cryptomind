using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Entities
{
    public abstract class Cipher
    {
        protected Cipher()
        {
            CipherTags = new List<CipherTag>();
            HintsRequested = new List<HintRequest>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(20),MinLength(3)]
        public string Title { get; set; }
        public string DecryptedText { get; set; }
        public string MLPrediction { get; set; }
        public string LLMAnalysis { get; set; }
        public CipherType TypeOfCipher { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool AllowHint { get; set; }
        public bool AllowSolution { get; set; }
        public bool IsApproved { get; set; }
        public int Points { get; set; }
        [ForeignKey(nameof(CreatedByUser))]
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public ICollection<CipherTag> CipherTags { get; set; }
        public ICollection<HintRequest> HintsRequested { get; set; }
        public ICollection<UserSolution> UsersSolved { get; set; }
    }
}