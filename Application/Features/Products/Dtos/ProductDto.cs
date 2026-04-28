using System.Text.Json.Serialization;
using Domain.Entities;

namespace Application.Features.Products.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool HasCategory { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? TrendMarkId { get; set; }
        public int? VendorId { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string YouTubeLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
