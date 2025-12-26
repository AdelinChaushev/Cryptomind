using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.UserViewModels
{
    public class AccountViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public int Points { get; set; }

        public int SolvedCount { get; set; }

        public string[] Roles { get; set; }

    }
}
