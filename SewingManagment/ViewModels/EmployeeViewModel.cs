using System.ComponentModel.DataAnnotations;

namespace SewingManagment.ViewModels
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "�m�W����")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "�~�֥���")]
        [Range(18, 100, ErrorMessage = "�~�֥����b 18~100 ��")]
        public required string Age { get; set; }

        [Required(ErrorMessage = "�ʧO����")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "¾�ȥ���")]
        public required string Position { get; set; }
    }
}
