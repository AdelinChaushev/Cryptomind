using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Entities
{
    public class CipherTag
    {
        [ForeignKey(nameof(Cipher))]
        public int CipherId { get; set; }
        public Cipher Cipher { get; set; }

        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
