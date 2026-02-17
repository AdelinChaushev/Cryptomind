using Cryptomind.Core.Services.OCR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Services
{
	public class OCRServiceTests
	{
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
		private readonly OCRService _service;

		public OCRServiceTests()
		{
			_httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			_configurationMock = new Mock<IConfiguration>();
			_configurationMock.Setup(c => c["OCRService:ApiUrl"])
				.Returns("http://localhost:5001");

			_service = new OCRService(
				_httpClientFactoryMock.Object,
				_configurationMock.Object);
		}

		private void SetupHttpResponse(HttpStatusCode statusCode, string content)
		{
			_httpMessageHandlerMock
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
			_httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.ThrowsAsync(exception);
		}

		private static Mock<IFormFile> CreateMockImageFile(
			string fileName = "test.jpg",
			long length = 1024)
		{
			var mock = new Mock<IFormFile>();
			mock.Setup(f => f.FileName).Returns(fileName);
			mock.Setup(f => f.Length).Returns(length);
			mock.Setup(f => f.ContentType).Returns("image/jpeg");
			mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("fake image data")));
			return mock;
		}

		private static string CreateOCRResponse(
			bool success = true,
			string text = "HELLO WORLD",
			double confidence = 0.95,
			int charCount = 11,
			bool? validationIsValid = true,
			string error = null)
		{
			var validation = validationIsValid.HasValue
				? new
				{
					is_valid = validationIsValid.Value,
					warnings = new string[] { },
					recommendation = "Good quality extraction"
				}
				: null;

			return JsonSerializer.Serialize(new
			{
				success = success,
				text = text,
				confidence = confidence,
				char_count = charCount,
				validation = validation,
				error = error
			});
		}

		#region ExtractTextFromImageAsync

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenImageFileIsNull()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _service.ExtractTextFromImageAsync(null));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenImageFileIsEmpty()
		{
			var imageFile = CreateMockImageFile(length: 0);

			await Assert.ThrowsAsync<ArgumentException>(
				() => _service.ExtractTextFromImageAsync(imageFile.Object));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_ReturnsResult_WhenExtractionSucceeds()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateOCRResponse());
			var imageFile = CreateMockImageFile();

			var result = await _service.ExtractTextFromImageAsync(imageFile.Object);

			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal("HELLO WORLD", result.ExtractedText);
			Assert.Equal(0.95, result.Confidence);
			Assert.Equal(11, result.CharCount);
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_PopulatesValidation_WhenPresent()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateOCRResponse());
			var imageFile = CreateMockImageFile();

			var result = await _service.ExtractTextFromImageAsync(imageFile.Object);

			Assert.NotNull(result.Validation);
			Assert.True(result.Validation.IsValid);
			Assert.Equal("Good quality extraction", result.Validation.Recommendation);
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_HandlesNullValidation()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateOCRResponse(validationIsValid: null));
			var imageFile = CreateMockImageFile();

			var result = await _service.ExtractTextFromImageAsync(imageFile.Object);

			Assert.NotNull(result.Validation);
			Assert.False(result.Validation.IsValid);
			Assert.Empty(result.Validation.Warnings);
			Assert.Empty(result.Validation.Recommendation);
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenApiReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "Server error");
			var imageFile = CreateMockImageFile();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ExtractTextFromImageAsync(imageFile.Object));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenResponseIsInvalidJson()
		{
			SetupHttpResponse(HttpStatusCode.OK, "not json");
			var imageFile = CreateMockImageFile();

			await Assert.ThrowsAsync<System.Text.Json.JsonException>(
				() => _service.ExtractTextFromImageAsync(imageFile.Object));
		}

		[Fact]
		public async Task ExtractTextFromImageAsync_Throws_WhenSuccessIsFalse()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateOCRResponse(
				success: false,
				error: "OCR processing failed"));
			var imageFile = CreateMockImageFile();

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ExtractTextFromImageAsync(imageFile.Object));

			Assert.Contains("OCR extraction failed", exception.Message);
		}

		#endregion

		#region ExtractTextWithMultipleMethodsAsync

		[Fact]
		public async Task ExtractTextWithMultipleMethodsAsync_Throws_WhenImageFileIsNull()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _service.ExtractTextWithMultipleMethodsAsync(null));
		}

		[Fact]
		public async Task ExtractTextWithMultipleMethodsAsync_Throws_WhenImageFileIsEmpty()
		{
			var imageFile = CreateMockImageFile(length: 0);

			await Assert.ThrowsAsync<ArgumentException>(
				() => _service.ExtractTextWithMultipleMethodsAsync(imageFile.Object));
		}

		[Fact]
		public async Task ExtractTextWithMultipleMethodsAsync_ReturnsResult_WhenExtractionSucceeds()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateOCRResponse());
			var imageFile = CreateMockImageFile();

			var result = await _service.ExtractTextWithMultipleMethodsAsync(imageFile.Object);

			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal("HELLO WORLD", result.ExtractedText);
		}

		[Fact]
		public async Task ExtractTextWithMultipleMethodsAsync_Throws_WhenApiReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.BadRequest, "Bad request");
			var imageFile = CreateMockImageFile();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ExtractTextWithMultipleMethodsAsync(imageFile.Object));
		}

		#endregion

		#region IsServiceHealthyAsync

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsTrue_WhenHealthEndpointReturnsSuccess()
		{
			SetupHttpResponse(HttpStatusCode.OK, "healthy");

			var result = await _service.IsServiceHealthyAsync();

			Assert.True(result);
		}

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsFalse_WhenHealthEndpointReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.ServiceUnavailable, "unhealthy");

			var result = await _service.IsServiceHealthyAsync();

			Assert.False(result);
		}

		[Fact]
		public async Task IsServiceHealthyAsync_ReturnsFalse_WhenExceptionOccurs()
		{
			SetupHttpException(new HttpRequestException("Connection failed"));

			var result = await _service.IsServiceHealthyAsync();

			Assert.False(result);
		}

		#endregion
	}
}