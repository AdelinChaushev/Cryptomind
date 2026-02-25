using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.Exceptions
{
    public class BannedException : Exception
    {
        public BannedException(string message) : base(message) { }
    }
}
