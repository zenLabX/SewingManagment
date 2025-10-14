using System.ComponentModel.DataAnnotations;

namespace SewingManagment.ViewModels
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "姓名必填")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "年齡必填")]
        [Range(18, 100, ErrorMessage = "年齡必須在 18~100 歲")]
        public required string Age { get; set; }

        [Required(ErrorMessage = "性別必填")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "職務必填")]
        public required string Position { get; set; }
    }
}
