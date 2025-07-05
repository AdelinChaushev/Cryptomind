using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Ciphers = new List<Cipher>();
            HintsRequested = new List<HintRequest>();
        }
        public int Score { get; set; }
        public int SolvedCount { get; set; }
        public ICollection<Cipher> Ciphers { get;}
        public ICollection<HintRequest> HintsRequested { get; set; }
    }
}
