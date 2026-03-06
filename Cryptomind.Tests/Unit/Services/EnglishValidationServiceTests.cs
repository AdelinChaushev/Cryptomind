using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class EnglishValidationServiceTests
	{
		private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
		private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
		private readonly EnglishValidationService service;

		public EnglishValidationServiceTests()
		{
			Environment.SetEnvironmentVariable("ML_URL", "http://localhost:5002");

			httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			var httpClient = new HttpClient(httpMessageHandlerMock.Object);

			httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			service = new EnglishValidationService(httpClientFactoryMock.Object);
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

		private static string CreateValidationResponse(bool isEnglish, double confidence)
		{
			return JsonSerializer.Serialize(new
			{
				is_english = isEnglish,
				confidence
			});
		}

		#region ValidatePlaintextAsync

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenPlaintextIsNull()
		{
			await Assert.ThrowsAsync<CustomValidationException>(() => service.ValidatePlaintextAsync(null));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenPlaintextIsEmpty()
		{
			await Assert.ThrowsAsync<CustomValidationException>(() => service.ValidatePlaintextAsync(""));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenPlaintextIsWhitespace()
		{
			await Assert.ThrowsAsync<CustomValidationException>(() => service.ValidatePlaintextAsync("   "));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_ReturnsResult_WhenResponseIsValid()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(true, 0.95));

			var result = await service.ValidatePlaintextAsync("Hello world");

			Assert.NotNull(result);
			Assert.True(result.IsEnglish);
			Assert.Equal(0.95, result.Confidence);
		}

		[Fact]
		public async Task ValidatePlaintextAsync_ReturnsNonEnglish_WhenTextIsNotEnglish()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(false, 0.15));

			var result = await service.ValidatePlaintextAsync("xqz jkl vwp");

			Assert.False(result.IsEnglish);
			Assert.Equal(0.15, result.Confidence);
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenApiReturnsNonSuccessStatusCode()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "Internal error");

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidatePlaintextAsync("test"));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenResponseIsInvalidJson()
		{
			SetupHttpResponse(HttpStatusCode.OK, "not json at all");

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidatePlaintextAsync("test"));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenHttpRequestFails()
		{
			SetupHttpException(new HttpRequestException("Connection refused"));

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidatePlaintextAsync("test"));
		}

		[Fact]
		public async Task ValidatePlaintextAsync_Throws_WhenRequestTimesOut()
		{
			SetupHttpException(new TaskCanceledException("Timeout"));

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidatePlaintextAsync("test"));
		}

		#endregion

		#region IsLikelyEnglishAsync

		[Fact]
		public async Task IsLikelyEnglishAsync_ReturnsTrue_WhenConfidenceAboveThreshold()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(true, 0.75));

			var result = await service.IsLikelyEnglishAsync("Hello world", minConfidence: 0.5);

			Assert.True(result);
		}

		[Fact]
		public async Task IsLikelyEnglishAsync_ReturnsFalse_WhenConfidenceBelowThreshold()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(true, 0.3));

			var result = await service.IsLikelyEnglishAsync("test", minConfidence: 0.5);

			Assert.False(result);
		}

		[Fact]
		public async Task IsLikelyEnglishAsync_ReturnsFalse_WhenIsEnglishIsFalse()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(false, 0.9));

			var result = await service.IsLikelyEnglishAsync("test", minConfidence: 0.5);

			Assert.False(result);
		}

		[Fact]
		public async Task IsLikelyEnglishAsync_UsesDefaultMinConfidence_WhenNotProvided()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(true, 0.6));

			var result = await service.IsLikelyEnglishAsync("Hello world");

			Assert.True(result);
		}

		[Fact]
		public async Task IsLikelyEnglishAsync_ReturnsTrue_WhenConfidenceEqualsThreshold()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidationResponse(true, 0.5));

			var result = await service.IsLikelyEnglishAsync("test", minConfidence: 0.5);

			Assert.True(result);
		}

		#endregion
	}
}