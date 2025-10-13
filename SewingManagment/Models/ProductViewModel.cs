using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "產品名稱必填")]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "產品代碼必填")]
        public required string ProductCode { get; set; }

        [Required(ErrorMessage = "價格必填")]
        public required string Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
