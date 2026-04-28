using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Properties")]
    public class Property : AuditEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int? ColorId { get; set; }
        public Color Color { get; set; }
        public int? SizeId { get; set; }
        public Size Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }
}
