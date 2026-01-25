using Cryptomind.Common.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IOCRService
	{
		Task<OCRResultDTO> ExtractTextFromImageAsync(IFormFile imageFile);
		Task<OCRResultDTO> ExtractTextWithMultipleMethodsAsync(IFormFile imageFile);
		Task<bool> IsServiceHealthyAsync();
	}
}
