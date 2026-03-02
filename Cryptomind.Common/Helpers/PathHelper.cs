namespace Cryptomind.Common.Helpers
{
	public static class PathHelper
	{
		public static string GetImagesBasePath()
		{
			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
				return Path.Combine(AppContext.BaseDirectory, "Images");
			else
				return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Images"));
		}
	}
}