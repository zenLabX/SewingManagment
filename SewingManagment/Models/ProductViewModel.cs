using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "���~�W�٥���")]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "���~�N�X����")]
        public required string ProductCode { get; set; }

        [Required(ErrorMessage = "���楲��")]
        public required string Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
