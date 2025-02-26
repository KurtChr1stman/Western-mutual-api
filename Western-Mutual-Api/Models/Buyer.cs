using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Western_Mutual_Api.Models
{
    [Table("Buyers")]
    public class Buyer
    {
        [Key]
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
