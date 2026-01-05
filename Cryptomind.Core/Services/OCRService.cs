using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class OCRService : IOCRService
	{
		public Task<OCRResultDTO> ExtractTextFromImageAsync(IFormFile imageFile)
		{
			throw new NotImplementedException();
		}

		public Task<OCRResultDTO> ExtractTextWithMultipleMethodsAsync(IFormFile imageFile)
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsServiceHealthyAsync()
		{
			throw new NotImplementedException();
		}
	}
}
