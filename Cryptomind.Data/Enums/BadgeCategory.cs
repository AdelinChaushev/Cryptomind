using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Enums
{
	public enum BadgeCategory
	{
		OnSolve,     // Triggered when user solves a cipher (includes points-based badges)
		OnUpload,    // Triggered when user uploads a cipher
		Periodic     // Checked by background job (complex criteria)
	}
}
