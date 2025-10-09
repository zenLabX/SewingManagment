using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Age { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        public string? Position { get; set; }
        public bool IsManager { get; set; }
    }
}
