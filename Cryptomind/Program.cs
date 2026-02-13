using Cryptomind.Core.Hubs;
using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Web.Extensions;
using Cryptomind.Core.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? SignalR is already added, but let's configure it properly for JWT
builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CryptomindDbContext>(options =>
	options.UseSqlServer(connectionString));

var auth = builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"]);

auth
	.AddCookie(c =>
	{
		c.Cookie.Name = "token";
	})
	.AddJwtBearer(options =>
	{
		options.SaveToken = true;
		options.RequireHttpsMetadata = false;

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidAudience = builder.Configuration["JWT:ValidAudience"],
			ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
			IssuerSigningKey = new SymmetricSecurityKey(key),
			RoleClaimType = ClaimTypes.Role
		};

		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				// Check cookie first
				context.Token = context.Request.Cookies["token"];

				// ? ADD THIS: Also check query string for SignalR connections
				// SignalR can't send custom headers, so it uses query string
				var accessToken = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;

				if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
				{
					context.Token = accessToken;
				}

				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.SignIn.RequireConfirmedAccount = false;
	options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<CryptomindDbContext>();

builder.Services.AddCors(c =>
{
	c.AddPolicy("AllowAll", builder =>
	{
		builder.WithOrigins("http://localhost:5173")
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});

builder.Services.RegisterRepositories();
builder.Services.RegisterUserDefinedServices();
builder.Services.AddHttpClient();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		await SeedRolesAndUsersAsync(services);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<BanCheckMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();

async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
{
	var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

	// Define roles
	string[] roleNames = { "Admin", "User" };

	foreach (var roleName in roleNames)
	{
		var roleExist = await roleManager.RoleExistsAsync(roleName);
		if (!roleExist)
		{
			await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}

	// Create default admin user
	var adminEmail = "admin@cryptomind.com";
	var adminUser = await userManager.FindByEmailAsync(adminEmail);

	if (adminUser == null)
	{
		var admin = new ApplicationUser
		{
			UserName = adminEmail,
			Email = adminEmail,
			EmailConfirmed = true
		};

		var result = await userManager.CreateAsync(admin, "Admin123!");

		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(admin, "Admin");
		}
	}

	var userEmail = "user@cryptomind.com";
	var userEntity = await userManager.FindByEmailAsync(userEmail);

	if (userEntity == null)
	{
		var user = new ApplicationUser
		{
			UserName = userEmail,
			Email = userEmail,
			EmailConfirmed = true
		};

		var result = await userManager.CreateAsync(user, "User123!");

		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(user, "User");
		}
	}
}