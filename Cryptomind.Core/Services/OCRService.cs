using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Cryptomind.Common.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services.OCR
{
	public class OCRService : IOCRService
	{
		private readonly HttpClient _httpClient;
		private readonly string _ocrApiUrl;

		private const int ApiTimeoutSeconds = 30;

		public OCRService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);

			_ocrApiUrl = configuration["OCRService:ApiUrl"] ?? "http://localhost:5001";
		}
		public async Task<OCRResultDTO> ExtractTextFromImageAsync(IFormFile imageFile)
		{
			if (imageFile == null || imageFile.Length == 0)
			{
				throw new ArgumentException("Image file cannot be empty", nameof(imageFile));
			}

			try
			{
				// Prepare multipart form data
				using var content = new MultipartFormDataContent();
				using var fileStream = imageFile.OpenReadStream();
				using var streamContent = new StreamContent(fileStream);

				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
				content.Add(streamContent, "image", imageFile.FileName);

				// Call Python OCR service
				var response = await _httpClient.PostAsync($"{_ocrApiUrl}/ocr/extract", content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new InvalidOperationException(
						$"OCR API returned error (Status {response.StatusCode}): {errorContent}"
					);
				}

				var responseJson = await response.Content.ReadAsStringAsync();

				var pythonResponse = JsonSerializer.Deserialize<PythonOCRResponse>(
					responseJson,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}
				);

				if (pythonResponse == null)
				{
					throw new InvalidOperationException("Failed to parse OCR API response");
				}

				if (!pythonResponse.Success)
				{
					throw new InvalidOperationException(
						$"OCR extraction failed: {pythonResponse.Error ?? "Unknown error"}"
					);
				}

				return ConvertToDTO(pythonResponse);
			}
			catch (HttpRequestException ex)
			{
				throw new InvalidOperationException(
					$"OCR service is unavailable. Please ensure the Python OCR API is running at {_ocrApiUrl}",
					ex
				);
			}
			catch (TaskCanceledException ex)
			{
				throw new InvalidOperationException(
					$"OCR service request timed out after {ApiTimeoutSeconds} seconds",
					ex
				);
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException(
					"Failed to parse response from OCR service. The service may be returning invalid data.",
					ex
				);
			}
			catch (InvalidOperationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					$"An unexpected error occurred during OCR extraction: {ex.Message}",
					ex
				);
			}
		}
		public async Task<OCRResultDTO> ExtractTextWithMultipleMethodsAsync(IFormFile imageFile)
		{
			if (imageFile == null || imageFile.Length == 0)
			{
				throw new ArgumentException("Image file cannot be empty", nameof(imageFile));
			}

			try
			{
				// Prepare multipart form data
				using var content = new MultipartFormDataContent();
				using var fileStream = imageFile.OpenReadStream();
				using var streamContent = new StreamContent(fileStream);

				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
				content.Add(streamContent, "image", imageFile.FileName);

				// Call Python OCR service with multiple methods endpoint
				var response = await _httpClient.PostAsync($"{_ocrApiUrl}/ocr/extract-multiple", content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new InvalidOperationException(
						$"OCR API returned error (Status {response.StatusCode}): {errorContent}"
					);
				}

				var responseJson = await response.Content.ReadAsStringAsync();

				var pythonResponse = JsonSerializer.Deserialize<PythonOCRResponse>(
					responseJson,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}
				);

				if (pythonResponse == null)
				{
					throw new InvalidOperationException("Failed to parse OCR API response");
				}

				if (!pythonResponse.Success)
				{
					throw new InvalidOperationException(
						$"OCR extraction with multiple methods failed: {pythonResponse.Error ?? "Unknown error"}"
					);
				}

				return ConvertToDTO(pythonResponse);
			}
			catch (HttpRequestException ex)
			{
				throw new InvalidOperationException(
					$"OCR service is unavailable. Please ensure the Python OCR API is running at {_ocrApiUrl}",
					ex
				);
			}
			catch (TaskCanceledException ex)
			{
				throw new InvalidOperationException(
					$"OCR service request timed out after {ApiTimeoutSeconds} seconds",
					ex
				);
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException(
					"Failed to parse response from OCR service. The service may be returning invalid data.",
					ex
				);
			}
			catch (InvalidOperationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					$"An unexpected error occurred during OCR extraction: {ex.Message}",
					ex
				);
			}
		}
		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_ocrApiUrl}/health");
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}
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

			[JsonPropertyName("method")]
			public string Method { get; set; }

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