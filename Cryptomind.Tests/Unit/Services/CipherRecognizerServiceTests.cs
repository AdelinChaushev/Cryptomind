using Cryptomind.Core.Services;
using Microsoft.Extensions.Configuration;
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
	public class CipherRecognizerServiceTests
	{
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
		private readonly CipherRecognizerService _service;

		public CipherRecognizerServiceTests()
		{
			_httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			_configurationMock = new Mock<IConfiguration>();
			_configurationMock.Setup(c => c["MLService:ApiUrl"])
				.Returns("http://localhost:5002");

			_service = new CipherRecognizerService(
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

		private static string CreateValidPythonResponse()
		{
			return JsonSerializer.Serialize(new
			{
				top_prediction = new
				{
					family = "Substitution",
					type = "Caesar",
					confidence = 0.95
				},
				all_predictions = new[]
				{
					new { family = "Substitution", type = "Caesar", confidence = 0.95 },
					new { family = "Substitution", type = "ROT13", confidence = 0.03 }
				}
			});
		}

		#region ClassifyCipher

		[Fact]
		public async Task ClassifyCipher_Throws_WhenInputIsNull()
		{
			await Assert.ThrowsAsync<ArgumentException>(() => _service.ClassifyCipher(null));
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenInputIsEmpty()
		{
			await Assert.ThrowsAsync<ArgumentException>(() => _service.ClassifyCipher(""));
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenInputIsWhitespace()
		{
			await Assert.ThrowsAsync<ArgumentException>(() => _service.ClassifyCipher("   "));
		}

		[Fact]
		public async Task ClassifyCipher_ReturnsResult_WhenResponseIsValid()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidPythonResponse());

			var result = await _service.ClassifyCipher("KHOOR ZRUOG");

			Assert.NotNull(result);
			Assert.Equal("Substitution", result.TopPrediction.Family);
			Assert.Equal("Caesar", result.TopPrediction.Type);
			Assert.Equal(0.95, result.TopPrediction.Confidence);
		}

		[Fact]
		public async Task ClassifyCipher_PopulatesAllPredictions_WhenPresent()
		{
			SetupHttpResponse(HttpStatusCode.OK, CreateValidPythonResponse());

			var result = await _service.ClassifyCipher("KHOOR ZRUOG");

			Assert.Equal(2, result.AllPredictions.Count);
			Assert.Equal("Caesar", result.AllPredictions[0].Type);
			Assert.Equal("ROT13", result.AllPredictions[1].Type);
		}

		[Fact]
		public async Task ClassifyCipher_ConvertsInputToUpperCase_BeforeSendingToAPI()
		{
			HttpRequestMessage capturedRequest = null;
			_httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(CreateValidPythonResponse())
				});

			await _service.ClassifyCipher("hello world");

			var requestBody = await capturedRequest.Content.ReadAsStringAsync();
			Assert.Contains("HELLO WORLD", requestBody);
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenApiReturnsNonSuccessStatusCode()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "Internal error");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ClassifyCipher("test"));
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenResponseIsInvalidJson()
		{
			SetupHttpResponse(HttpStatusCode.OK, "not json at all");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ClassifyCipher("test"));
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenResponseMissingTopPrediction()
		{
			var invalidResponse = JsonSerializer.Serialize(new
			{
				all_predictions = new[]
				{
					new { family = "Substitution", type = "Caesar", confidence = 0.95 }
				}
			});
			SetupHttpResponse(HttpStatusCode.OK, invalidResponse);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ClassifyCipher("test"));
		}

		[Fact]
		public async Task ClassifyCipher_ReturnsEmptyAllPredictions_WhenNotPresent()
		{
			var responseWithoutAll = JsonSerializer.Serialize(new
			{
				top_prediction = new
				{
					family = "Substitution",
					type = "Caesar",
					confidence = 0.95
				}
			});
			SetupHttpResponse(HttpStatusCode.OK, responseWithoutAll);

			var result = await _service.ClassifyCipher("test");

			Assert.Empty(result.AllPredictions);
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenHttpRequestFails()
		{
			SetupHttpException(new HttpRequestException("Connection refused"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ClassifyCipher("test"));
		}

		[Fact]
		public async Task ClassifyCipher_Throws_WhenRequestTimesOut()
		{
			SetupHttpException(new TaskCanceledException("Timeout"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ClassifyCipher("test"));
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
			SetupHttpResponse(HttpStatusCode.InternalServerError, "unhealthy");

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