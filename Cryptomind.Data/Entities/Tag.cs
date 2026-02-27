using Cryptomind.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptomind.Data.Entities
{
	public class Tag
	{
		public Tag()
		{
			CipherTags = new List<CipherTag>();
		}
		[Key]
		public int Id { get; set; }
		public TagType Type { get; set; }
		public ICollection<CipherTag> CipherTags { get; set; }

	}
}
