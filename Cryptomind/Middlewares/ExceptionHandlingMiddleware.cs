using Cryptomind.Common.Exceptions;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionHandlingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (BannedException ex)
		{
			await WriteResponse(context, StatusCodes.Status403Forbidden, ex.Message);
		}
		catch (NotFoundException ex)
		{
			await WriteResponse(context, StatusCodes.Status404NotFound, ex.Message);
		}
		catch (ConflictException ex)
		{
			await WriteResponse(context, StatusCodes.Status409Conflict, ex.Message);
		}
		catch (UnauthorizedException ex)
		{
			await WriteResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
		}
		catch (CustomValidationException ex)
		{
			await WriteResponse(context, StatusCodes.Status400BadRequest, ex.Message);
		}
		catch (Exception)
		{
			await WriteResponse(context, StatusCodes.Status500InternalServerError, "Възникна неочаквана грешка");
		}
	}

	private static async Task WriteResponse(HttpContext context, int statusCode, string message)
	{
		context.Response.StatusCode = statusCode;
		context.Response.ContentType = "application/json";
		var body = JsonSerializer.Serialize(new { error = message });
		await context.Response.WriteAsync(body);
	}
}