using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Services;
using Cryptomind.Data.Enums;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class LLMServiceTests
	{
		private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
		private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
		private readonly LLMService service;

		public LLMServiceTests()
		{
			Environment.SetEnvironmentVariable("OPENAI_API_URL", "https://api.openai.com/v1");
			Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-api-key");
			Environment.SetEnvironmentVariable("VALIDATION_MODEL", "gpt-4o-mini");
			Environment.SetEnvironmentVariable("EDUCATIONAL_MODEL", "gpt-4o");

			httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			var httpClient = new HttpClient(httpMessageHandlerMock.Object);

			httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			service = new LLMService(httpClientFactoryMock.Object);
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

		private static string CreateOpenAIResponse(string messageContent)
		{
			return JsonSerializer.Serialize(new
			{
				choices = new[]
				{
					new
					{
						message = new
						{
							content = messageContent
						}
					}
				}
			});
		}

		private static string CreateValidationResultJson(
			string predictedType = "Caesar",
			string confidence = "high",
			bool? solutionCorrect = true,
			bool isAppropriate = true,
			string recommendation = "approve")
		{
			var result = new
			{
				predicted_type = predictedType,
				confidence,
				solution_correct = solutionCorrect,
				is_appropriate = isAppropriate,
				issues = new string[] { },
				recommendation,
				reasoning = "Test reasoning"
			};
			return JsonSerializer.Serialize(result);
		}

		private static CipherRecognitionResultViewModel CreateMLResult(
			string type = "Caesar",
			double confidence = 0.95)
		{
			return new CipherRecognitionResultViewModel
			{
				TopPrediction = new PredictionViewModel
				{
					Type = type,
					Confidence = confidence
				},
				AllPredictions = new List<PredictionViewModel>
				{
					new() { Type = type, Confidence = confidence }
				}
			};
		}

		#region ValidateCipherAsync - Case 1 (Type + Solution)

		[Fact]
		public async Task ValidateCipherAsync_ReturnsResult_WhenBothTypeAndSolutionProvided()
		{
			var validationJson = CreateValidationResultJson();
			var llmResponse = CreateOpenAIResponse(validationJson);
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var mlResult = CreateMLResult();
			var result = await service.ValidateCipherAsync(
				"KHOOR", "HELLO", mlResult, "Caesar");

			Assert.NotNull(result);
			Assert.Equal("Caesar", result.PredictedType);
			Assert.Equal("high", result.Confidence);
			Assert.True(result.SolutionCorrect);
			Assert.True(result.IsAppropriate);
			Assert.Equal("approve", result.Recommendation);
		}

		#endregion

		#region ValidateCipherAsync - Case 2 (Type Only)

		[Fact]
		public async Task ValidateCipherAsync_ReturnsResult_WhenOnlyTypeProvided()
		{
			var validationJson = CreateValidationResultJson(solutionCorrect: null);
			var llmResponse = CreateOpenAIResponse(validationJson);
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var mlResult = CreateMLResult();
			var result = await service.ValidateCipherAsync(
				"KHOOR", null, mlResult, "Caesar");

			Assert.NotNull(result);
			Assert.Equal("Caesar", result.PredictedType);
			Assert.Null(result.SolutionCorrect);
		}

		#endregion

		#region ValidateCipherAsync - Case 3 (Solution Only)

		[Fact]
		public async Task ValidateCipherAsync_ReturnsResult_WhenOnlySolutionProvided()
		{
			var validationJson = CreateValidationResultJson();
			var llmResponse = CreateOpenAIResponse(validationJson);
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var mlResult = CreateMLResult();
			var result = await service.ValidateCipherAsync(
				"KHOOR", "HELLO", mlResult, null);

			Assert.NotNull(result);
			Assert.Equal("Caesar", result.PredictedType);
			Assert.True(result.SolutionCorrect);
		}

		#endregion

		#region ValidateCipherAsync - Error Handling

		[Fact]
		public async Task ValidateCipherAsync_ReturnsFallback_WhenJsonParsingFails()
		{
			var llmResponse = CreateOpenAIResponse("not valid json");
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var mlResult = CreateMLResult();
			var result = await service.ValidateCipherAsync(
				"KHOOR", "HELLO", mlResult, "Caesar");

			Assert.NotNull(result);
			Assert.Equal("reject", result.Recommendation);
			Assert.Single(result.Issues);
			Assert.Contains("Неуспешно разчитане на отговора от LLM — необходим е ръчен преглед.", result.Issues[0]);
		}

		[Fact]
		public async Task ValidateCipherAsync_Throws_WhenApiReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.InternalServerError, "Server error");

			var mlResult = CreateMLResult();

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidateCipherAsync("KHOOR", "HELLO", mlResult, "Caesar"));
		}

		[Fact]
		public async Task ValidateCipherAsync_Throws_WhenApiResponseHasNoChoices()
		{
			var emptyResponse = JsonSerializer.Serialize(new { choices = new object[] { } });
			SetupHttpResponse(HttpStatusCode.OK, emptyResponse);

			var mlResult = CreateMLResult();

			await Assert.ThrowsAsync<Exception>(
				() => service.ValidateCipherAsync("KHOOR", "HELLO", mlResult, "Caesar"));
		}

		#endregion

		#region GetHint

		[Fact]
		public async Task GetHint_ReturnsTypeHint_WhenHintTypeIsType()
		{
			var llmResponse = CreateOpenAIResponse("This is a Caesar cipher.");
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var cipher = new ConcreteCipher
			{
				EncryptedText = "KHOOR",
				DecryptedText = "HELLO",
				TypeOfCipher = CipherType.Caesar
			};

			var result = await service.GetHint(cipher, HintType.Type);

			Assert.Equal("This is a Caesar cipher.", result);
		}

		[Fact]
		public async Task GetHint_ReturnsSolutionHint_WhenHintTypeIsHint()
		{
			var llmResponse = CreateOpenAIResponse("Look for letter frequency patterns.");
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var cipher = new ConcreteCipher
			{
				EncryptedText = "KHOOR",
				DecryptedText = "HELLO",
				TypeOfCipher = CipherType.Caesar
			};

			var result = await service.GetHint(cipher, HintType.Hint);

			Assert.Equal("Look for letter frequency patterns.", result);
		}

		[Fact]
		public async Task GetHint_ReturnsFullSolution_WhenHintTypeIsFullSolution()
		{
			var llmResponse = CreateOpenAIResponse("The plaintext is HELLO with shift 3.");
			SetupHttpResponse(HttpStatusCode.OK, llmResponse);

			var cipher = new ConcreteCipher
			{
				EncryptedText = "KHOOR",
				DecryptedText = "HELLO",
				TypeOfCipher = CipherType.Caesar
			};

			var result = await service.GetHint(cipher, HintType.FullSolution);

			Assert.Equal("The plaintext is HELLO with shift 3.", result);
		}

		[Fact]
		public async Task GetHint_Throws_WhenApiReturnsError()
		{
			SetupHttpResponse(HttpStatusCode.BadRequest, "Bad request");

			var cipher = new ConcreteCipher
			{
				EncryptedText = "KHOOR",
				DecryptedText = "HELLO",
				TypeOfCipher = CipherType.Caesar
			};

			await Assert.ThrowsAsync<Exception>(
				() => service.GetHint(cipher, HintType.Type));
		}

		#endregion

		#region Constructor

		[Fact]
		public void Constructor_Throws_WhenApiUrlNotConfigured()
		{
			Environment.SetEnvironmentVariable("OPENAI_API_URL", null);

			Assert.Throws<Exception>(
				() => new LLMService(httpClientFactoryMock.Object));

			Environment.SetEnvironmentVariable("OPENAI_API_URL", "https://api.openai.com/v1");
		}

		[Fact]
		public void Constructor_Throws_WhenApiKeyNotConfigured()
		{
			Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

			Assert.Throws<Exception>(
				() => new LLMService(httpClientFactoryMock.Object));

			Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-api-key");
		}

		#endregion
	}
}