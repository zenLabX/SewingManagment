using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Models
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "�m�W����")]
        public string Name { get; set; }

        [Required(ErrorMessage = "�~�֥���")]
        [Range(18, 100, ErrorMessage = "�~�֥����b 18~100 ��")]
        public string Age { get; set; }

        [Required(ErrorMessage = "�ʧO����")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "¾�ȥ���")]
        public string Position { get; set; }
    }
}
