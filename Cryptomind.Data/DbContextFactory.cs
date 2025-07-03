using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data
{
   
    
        public class CryptomindDbContextFactory : IDesignTimeDbContextFactory<CryptomindDbContext>
        {
            public CryptomindDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<CryptomindDbContext>();

                // Use your actual connection string here
                optionsBuilder.UseSqlServer("Server=SAMUIL\\SQLEXPRESS;Database=CryptomindDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

                return new CryptomindDbContext(optionsBuilder.Options);
            }
        }
    
}
