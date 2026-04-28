using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool HasSubCategories { get; set; }
    }
}
