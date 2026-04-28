using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Dtos
{
    public class CreateUpdateProductDto
    {
        [Required]
        public string NameEn { get; set; }
        [Required]
        public string NameAr { get; set; }
        public bool HasCategory { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        [Required]
        public int? TrendMarkId { get; set; }
        [Required]
        public int? VendorId { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string YouTubeLink { get; set; }
        
        // This will be bound from form as JSON string
        public string? PropertiesJson { get; set; }
        
        public IList<CreatePropertyDto> Properties { get; set; } = new List<CreatePropertyDto>();
    }
}
