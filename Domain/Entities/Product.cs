using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    [Table("Products")]
    public class Product : AuditEntity
    {
        public int Id { get; set; }
        
        // Names
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        
        // Category & SubCategory
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public int? SubCategoryId { get; set; }
        public ProductCategory SubCategory { get; set; }
        
        // TrendMark
        public int? TrendMarkId { get; set; }
        public TrendMark TrendMark { get; set; }
        
        // Vendor (User)
        public int VendorId { get; set; }
        public User Vendor { get; set; }
        
        // Pricing
        public decimal ? Price { get; set; }
        public decimal ? PriceAfterDiscount { get; set; }
        
        // Image
        public string ? Image { get; set; }
        
        // Quantity
        public int ? Quantity { get; set; }
        
        // Descriptions
        public string ? DescriptionAr { get; set; }
        public string ? DescriptionEn { get; set; }
        
        // YouTube
        public string ? YouTubeLink { get; set; }
        
        // Properties (Color, Size variations)
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
