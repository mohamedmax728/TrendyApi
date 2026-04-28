using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Abstractions
{
    public interface IImageService
    {
        Task<BaseResponse<List<string>>> UploadImagesAsync(List<IFormFile> imageFiles);
        Task<BaseResponse> DeleteImageAsync(string imageUrl);
    }
}
