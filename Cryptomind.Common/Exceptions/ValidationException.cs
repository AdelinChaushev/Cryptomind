using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.Exceptions
{
	public class CustomValidationException : Exception
	{
		public CustomValidationException(string message) : base(message) { }
	}
}
