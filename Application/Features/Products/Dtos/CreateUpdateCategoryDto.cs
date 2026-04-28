using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Dtos
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        public string NameEn { get; set; }
        [Required]
        public string NameAr { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool HasSubCategories { get; set; }
    }
}
