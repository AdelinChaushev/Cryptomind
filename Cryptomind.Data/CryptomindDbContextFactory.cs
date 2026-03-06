using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cryptomind.Data
{
	public class CryptomindDbContextFactory : IDesignTimeDbContextFactory<CryptomindDbContext>
	{
		public CryptomindDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<CryptomindDbContext>();

			// Use the actual connection string here
			optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CryptomindDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True");

			return new CryptomindDbContext(optionsBuilder.Options);
		}
	}

}