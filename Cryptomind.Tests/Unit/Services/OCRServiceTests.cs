using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Services.OCR;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class OCRServiceTests
	{
		private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
		private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
		private readonly OCRService service;

		public OCRServiceTests()
		{
			Environment.SetEnvironmentVariable("OCR_URL", "http://localhost:5001");

			httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			var httpClient = new HttpClient(httpMessageHandlerMock.Object);

			httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			service = new OCRService(httpClientFactoryMock.Object);
		}

		private void SetupHttpResponse(HttpStatusCode statusCode, string content)
		{
			httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = statusCode,
					Content = new StringContent(content)
				});
		}

		private void SetupHttpException(Exception exception)
		{
			httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.ThrowsAsync(exception);
		}

		private static string CreateSuccessResponse(
			string text = "KHOOR ZRUOG",
			double confidence = 0.95,
			int charCount = 11,
			bool success = true,
			bool validationIsValid = true,
			string? error = null)
		{
			return JsonSerializer.Serialize(new
			{
				success,
				text,
				confidence,
				char_count = charCount,
				validation = new
				{
					is_valid = validationIsValid,
					warnings = Array.Empty<string>(),
					recommendation = "looks good"
				},
				error
			});
		}

		private static IFormFile MakeFormFile(string content = "fake image content", string fileName = "test.jpg", string contentType = "image/jpeg")
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(content);
			var stream = new MemoryStream(bytes);
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns(fileName);
			fileMock.Setup(f => f.ContentType).Returns(contentType);
			fileMock.Setup(f => f.Length).Returns(bytes.Length);
			fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
			return fileMock.Object;
		}

		private static IFormFile MakeEmptyFormFile()
		{
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.Length).Returns(0);
			return fileMock.Object;
		}

		#region ExtractTextFromImageAsync

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenImageIsNull()
		{
			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.ExtractTextFromImageAsync(null));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenImageIsEmpty()
		{
			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.ExtractTextFromImageAsync(MakeEmptyFormFile()));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_ReturnsResult_WhenResponseIsValid()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateSuccessResponse(text: "KHOOR ZRUOG", confidence: 0.95));

			var result = await service.ExtractTextFromImageAsync(MakeFormFile());

			Assert.NotNull(result);
			Assert.Equal("KHOOR ZRUOG", result.ExtractedText);
			Assert.Equal(0.95, result.Confidence);
			Assert.True(result.Success);
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenApiReturnsNonSuccessStatusCode()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "error");

			await Assert.ThrowsAsync<Exception>(
				() => service.ExtractTextFromImageAsync(MakeFormFile()));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenPythonResponseSuccessIsFalse()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateSuccessResponse(success: false, error: "OCR failed"));

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.ExtractTextFromImageAsync(MakeFormFile()));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_WrapsException_WhenHttpRequestFails()
		{
			SetupHttpException(new HttpRequestException("Connection refused"));

			var ex = await Assert.ThrowsAsync<Exception>(
				() => service.ExtractTextFromImageAsync(MakeFormFile()));

			Assert.IsType<HttpRequestException>(ex.InnerException);
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_WrapsException_WhenRequestTimesOut()
		{
			SetupHttpException(new TaskCanceledException("Timeout"));

			var ex = await Assert.ThrowsAsync<Exception>(
				() => service.ExtractTextFromImageAsync(MakeFormFile()));

			Assert.IsType<TaskCanceledException>(ex.InnerException);
		}

		#endregion

		#region IsServiceHealthyAsync

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsTrue_WhenHealthEndpointReturnsSuccess()
		{
			SetupHttpResponse(HttpStatusCode.OK, "healthy");

			var result = await service.IsServiceHealthyAsync();

			Assert.True(result);
		}

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsFalse_WhenHealthEndpointReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "unhealthy");

			var result = await service.IsServiceHealthyAsync();

			Assert.False(result);
		}

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsFalse_WhenExceptionOccurs()
		{
			SetupHttpException(new HttpRequestException("Connection failed"));

			var result = await service.IsServiceHealthyAsync();

			Assert.False(result);
		}

		#endregion
	}
}