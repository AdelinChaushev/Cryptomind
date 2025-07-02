using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data
{
    public class CryptomindDbContext :IdentityDbContext<ApplicationUser>
    {

        public CryptomindDbContext(DbContextOptions<CryptomindDbContext> options)
        : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<CipherTag> CipherTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<HintRequest> HintRequests { get; set; }
        public DbSet<ImageCipher> ImageCiphers { get; set; }
        public DbSet<TextCipher> TextCiphers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CipherTag>()
                .HasKey(c => new { c.CipherId, c.TagId });

            builder.Entity<HintRequest>().HasOne(c => c.Cipher)
               .WithMany(c => c.HintsRequested).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<HintRequest>().HasOne(c => c.ApplicationUser)
               .WithMany(c => c.HintsRequested).OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }

    }
}
