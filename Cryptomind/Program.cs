using Cryptomind.Core.Hubs;
using Cryptomind.Core.Middlewares;
using Cryptomind.Data;
using Cryptomind.Web.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
	options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();


builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.RegisterRepositories();
builder.Services.RegisterUserDefinedServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseMiddleware<BanCheckMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
	if (db.Database.IsRelational())
	{
		db.Database.Migrate();
	}
}

app.Run();

public partial class Program { }