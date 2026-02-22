using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Data
{
	public class CryptomindDbContextFactory : IDesignTimeDbContextFactory<CryptomindDbContext>
	{
		public CryptomindDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<CryptomindDbContext>();

			//DON'T FORGET TO USE CONNECTION STRING HERE TOO.
			optionsBuilder.UseSqlServer("Server=.;Database=CryptomindDb;Trusted_Connection=True;TrustServerCertificate=True;");

			return new CryptomindDbContext(optionsBuilder.Options);
		}
	}
}
