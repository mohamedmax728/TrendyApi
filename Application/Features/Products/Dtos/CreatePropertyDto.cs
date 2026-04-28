using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Dtos
{
    public class CreatePropertyDto
    {
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }
}
