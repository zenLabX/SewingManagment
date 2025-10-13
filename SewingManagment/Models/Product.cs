using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string ProductCode { get; set; } = string.Empty;
        [Required]
        public string Price { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
