using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Cryptomind.Core.Services.OCR
{
	public class OCRService : IOCRService
	{
		private readonly HttpClient httpClient;
		private readonly string ocrApiUrl;

		private const int ApiTimeoutSeconds = OCRServiceConstants.ApiTimeoutSeconds;

		public OCRService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			httpClient = httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);
			ocrApiUrl = configuration["OCRService:ApiUrl"] ?? "http://localhost:5001";
		}
		public async Task<OCRResultDTO> ExtractTextFromImageAsync(IFormFile imageFile)
		{
			if (imageFile == null || imageFile.Length == 0)
				throw new CustomValidationException(CipherErrorConstants.ImageRequired);
			try
			{
				var result = await SendOCRRequestAsync(imageFile, "ocr/extract");

				if (string.IsNullOrEmpty(result.ExtractedText))
					throw new Exception();

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"Услугата за OCR не е налична. Моля, уверете се, че Python OCR работи на {ocrApiUrl}",
					ex
				);
			}
		}
		public async Task<OCRResultDTO> ExtractTextWithMultipleMethodsAsync(IFormFile imageFile)
		{
			if (imageFile == null || imageFile.Length == 0)
				throw new CustomValidationException(CipherErrorConstants.ImageRequired);

			return await SendOCRRequestAsync(imageFile, "ocr/extract-multiple");
		}
		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				var response = await httpClient.GetAsync($"{ocrApiUrl}/health");
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}

		#region Private methods
		private OCRResultDTO ConvertToDTO(PythonOCRResponse pythonResponse)
		{
			return new OCRResultDTO
			{
				Success = pythonResponse.Success,
				ExtractedText = pythonResponse.Text ?? string.Empty,
				Confidence = pythonResponse.Confidence,
				CharCount = pythonResponse.CharCount,
				Validation = ConvertToValidationDTO(pythonResponse.Validation),
				ErrorMessage = pythonResponse.Error
			};
		}
		private OCRValidationDTO ConvertToValidationDTO(PythonValidation pythonValidation)
		{
			if (pythonValidation == null)
			{
				return new OCRValidationDTO
				{
					IsValid = false,
					Warnings = new List<string>(),
					Recommendation = string.Empty
				};
			}

			return new OCRValidationDTO
			{
				IsValid = pythonValidation.IsValid,
				Warnings = pythonValidation.Warnings ?? new List<string>(),
				Recommendation = pythonValidation.Recommendation ?? string.Empty
			};
		}
		private async Task<OCRResultDTO> SendOCRRequestAsync(IFormFile imageFile, string endpoint)
		{
			using var content = new MultipartFormDataContent();
			using var fileStream = imageFile.OpenReadStream();
			using var streamContent = new StreamContent(fileStream);

			streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
			content.Add(streamContent, "image", imageFile.FileName);

			var response = await httpClient.PostAsync($"{ocrApiUrl}/{endpoint}", content);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new Exception($"OCR API върна грешка (Статус {response.StatusCode}): {errorContent}");
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			var pythonResponse = JsonSerializer.Deserialize<PythonOCRResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (pythonResponse == null)
				throw new Exception("Неуспешен анализ на отговора на OCR API");

			if (!pythonResponse.Success)
				throw new Exception($"OCR извличането е неуспешно: {pythonResponse.Error ?? "Неизвестна грешка"}");

			return ConvertToDTO(pythonResponse);
		}
		#endregion

		#region Internal Models for Python API
		private class PythonOCRResponse
		{
			[JsonPropertyName("success")]
			public bool Success { get; set; }

			[JsonPropertyName("text")]
			public string Text { get; set; }

			[JsonPropertyName("confidence")]
			public double Confidence { get; set; }

			[JsonPropertyName("char_count")]
			public int CharCount { get; set; }

			[JsonPropertyName("validation")]
			public PythonValidation Validation { get; set; }

			[JsonPropertyName("error")]
			public string Error { get; set; }
		}
		private class PythonValidation
		{
			[JsonPropertyName("is_valid")]
			public bool IsValid { get; set; }

			[JsonPropertyName("warnings")]
			public List<string> Warnings { get; set; }

			[JsonPropertyName("recommendation")]
			public string Recommendation { get; set; }
		}
		#endregion
	}
}