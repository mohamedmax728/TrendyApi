using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductCategories")]
    public class ProductCategory : AuditEntity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Description { get; set; }
        
        // Parent category (optional - for subcategories)
        public int? ParentId { get; set; }
        public ProductCategory Parent { get; set; }
        
        // Indicates if this category has subcategories
        public bool HasSubCategories { get; set; }
    }
}
