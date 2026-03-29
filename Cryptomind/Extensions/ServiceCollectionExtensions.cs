using Cryptomind.Core.Contracts;
using Cryptomind.Core.Rooms;
using Cryptomind.Core.Services;
using Cryptomind.Core.Services.OCR;
using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

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
		services.AddScoped<IRepository<DailyChallengeEntry, int>, Repository<DailyChallengeEntry, int>>();
		services.AddScoped<IRepository<DailyChallengeParticipation, int>, Repository<DailyChallengeParticipation, int>>();

		return services;
	}

	public static IServiceCollection RegisterUserDefinedServices(
		this IServiceCollection services)
	{
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICipherService, CipherService>();
		services.AddScoped<IAdminCipherService, AdminCipherService>();
		services.AddScoped<IAdminAnswerService, AdminAnswerService>();
		services.AddScoped<IAdminUserService, AdminUserService>();
		services.AddScoped<ICipherRecognizerService, CipherRecognizerService>();
		services.AddScoped<IOCRService, OCRService>();
		services.AddScoped<IBadgeService, BadgeService>();
		services.AddScoped<IBadgeStatisticsService, BadgeStatisticsService>();
		services.AddScoped<ILLMService, LLMService>();
		services.AddScoped<IEnglishValidationService, EnglishValidationService>();
		services.AddScoped<IHintService, HintService>();
		services.AddScoped<INotificationService, NotificationService>();
		services.AddScoped<ICipherSubmissionService, CipherSubmissionService>();
		services.AddScoped<IAnswerSubmissionService, AnswerSubmissionService>();
		services.AddScoped<ILeaderboardService, LeaderboardService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IRoomService, RoomService>();
		services.AddScoped<IDailyChallengeService, DailyChallengeService>();

		services.AddSingleton<RoomStore>();

		return services;
	}
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<CryptomindDbContext>(options =>
			options.UseSqlServer(connectionString, sqlOptions =>
				sqlOptions.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(10),
					errorNumbersToAdd: null)));
		return services;
	}
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddCookie(c => c.Cookie.Name = "token")
		.AddJwtBearer(options =>
		{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
				ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
				IssuerSigningKey = new SymmetricSecurityKey(key),
				RoleClaimType = ClaimTypes.Role
			};
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					context.Token = context.Request.Cookies["token"];
					var accessToken = context.Request.Query["access_token"];
					var path = context.HttpContext.Request.Path;
					if (!string.IsNullOrEmpty(accessToken) &&
						(path.StartsWithSegments("/notificationHub") || path.StartsWithSegments("/raceRoomHub")))
						context.Token = accessToken;
					return Task.CompletedTask;
				}
			};
		});

		return services;
	}
	public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
	{
		services.AddIdentity<ApplicationUser, IdentityRole>(options =>
		{
			options.Password.RequiredLength = 6;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireDigit = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireLowercase = false;
			options.SignIn.RequireConfirmedAccount = false;
			options.User.RequireUniqueEmail = true;
		}).AddEntityFrameworkStores<CryptomindDbContext>();

		return services;
	}
	public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
	{
		services.AddCors(c =>
		{
			c.AddPolicy("AllowAll", builder =>
			{
				builder.WithOrigins("http://localhost:5173", "http://localhost:5174", "https://cryptomind.techlab.cloud")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
			});
		});

		return services;
	}
}
