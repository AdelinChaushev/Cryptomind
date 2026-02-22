using Cryptomind.Common.DTOs;
using Microsoft.AspNetCore.Http;

namespace Cryptomind.Core.Contracts
{
	public interface IOCRService
	{
		Task<OCRResultDTO> ExtractTextFromImageAsync(IFormFile imageFile);
		Task<OCRResultDTO> ExtractTextWithMultipleMethodsAsync(IFormFile imageFile);
		Task<bool> IsServiceHealthyAsync();
	}
}
