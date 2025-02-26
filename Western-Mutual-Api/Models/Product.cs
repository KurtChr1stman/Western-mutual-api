using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Western_Mutual_Api.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public required string SKU { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string BuyerId { get; set; }  
        public required bool Active { get; set; }
    }
}
