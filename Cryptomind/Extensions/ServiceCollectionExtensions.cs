using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Core.Services.OCR;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;

namespace Cryptomind.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IRepository<Cipher, int>, Repository<Cipher, int>>();
        services.AddScoped<IRepository<TextCipher, int>, Repository<TextCipher, int>>();
        services.AddScoped<IRepository<ImageCipher, int>, Repository<ImageCipher, int>>();
        services.AddScoped<IRepository<Tag, int>, Repository<Tag, int>>();
        services.AddScoped<IRepository<HintRequest, int>, Repository<HintRequest, int>>();
        services.AddScoped<IRepository<UserSolution, int>, Repository<UserSolution, int>>();
		services.AddScoped<IRepository<AnswerSuggestion, int>, Repository<AnswerSuggestion, int>>();
		services.AddScoped<IRepository<ApplicationUser, string>, Repository<ApplicationUser, string>>();
		services.AddScoped<IRepository<UserBadge, int>, Repository<UserBadge, int>>();
		services.AddScoped<IRepository<Badge, int>, Repository<Badge, int>>();
		services.AddScoped<IRepository<Notification, int>, Repository<Notification, int>>();

		return services;
    }

    public static IServiceCollection RegisterUserDefinedServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICipherService, CipherService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICipherRecognizerService, CipherRecognizerService>();
		services.AddScoped<IOCRService, OCRService>();
        services.AddScoped<IBadgeService, BadgeService>();
		services.AddScoped<IBadgeStatisticsService, BadgeStatiscticsService>();
        services.AddScoped<ILLMService, LLMService>();
        services.AddScoped<IEnglishValidationService, EnglishValidationService>();
        services.AddScoped<IHintService, HintService>();
		services.AddScoped<INotificationService, NotificationService>();
		services.AddScoped<ICipherSubmissionService, CipherSubmitionService>();
		services.AddScoped<IAnswerService, AnswerService>();

		return services;
    }
}
