

using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Crytomind.Core.Contracts;
using Crytomind.Core.Services;

namespace ClarifEye.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IRepository<TextCipher, int>, Repository<TextCipher, int>>();
        services.AddScoped<IRepository<ImageCipher, int>, Repository<ImageCipher, int>>();
        services.AddScoped<IRepository<Tag, string>, Repository<Tag, string>>();
        services.AddScoped<IRepository<HintRequest, int>, Repository<HintRequest, int>>();

        return services;
    }

    public static IServiceCollection RegisterUserDefinedServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICipherService, CipherService>();
        services.AddScoped<IAdminService, AdminService>();

		return services;
    }
}
