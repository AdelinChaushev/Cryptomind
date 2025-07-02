using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
