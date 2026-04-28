using Application.Common.Abstractions;
using Application.Common.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace TrendyApi.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<BaseResponse<List<string>>> UploadImagesAsync(List<IFormFile> imageFiles)
        {
            try
            {
                var imageUrls = new List<string>();
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads", "products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var imageFile in imageFiles)
                {
                    if (imageFile == null || imageFile.Length == 0)
                        continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imageUrls.Add($"/uploads/products/{fileName}");
                }

                return new BaseResponse<List<string>>
                {
                    Success = true,
                    Message = "Images uploaded successfully.",
                    Data = imageUrls
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<string>>
                {
                    Success = false,
                    Message = "Image upload failed.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return new BaseResponse { Success = false, Message = "Image URL is required." };

                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", "products", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return new BaseResponse { Success = true, Message = "Image deleted successfully." };
                }

                return new BaseResponse { Success = false, Message = "Image not found." };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Image deletion failed.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }
    }
}
