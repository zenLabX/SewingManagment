using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "姓名必填")]
        public string Name { get; set; }

        [Required(ErrorMessage = "年齡必填")]
        [Range(18, 100, ErrorMessage = "年齡必須在 18~100 歲")]
        public string Age { get; set; }

        [Required(ErrorMessage = "性別必填")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "職務必填")]
        public string Position { get; set; }
    }
}
