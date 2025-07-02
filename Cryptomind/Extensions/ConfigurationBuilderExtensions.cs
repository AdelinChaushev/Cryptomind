namespace ClarifEye.Web.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEnvironmentSpecificJsonFiles(
        this IConfigurationBuilder builder,
        IWebHostEnvironment environment,
        out string connectionString)
    {
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        if (environment.EnvironmentName == "Development")
        {
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            connectionString = builder.Build().GetConnectionString("DevConnection")!;
        }
        else
        {
            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
            connectionString = builder.Build().GetConnectionString("ProdConnection")!;
        }

        return builder;
    }
}
