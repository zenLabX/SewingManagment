using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Enums
{
    public enum EmployeeSearchField
    {
        [Display(Name = "姓名")]
        Name,
        [Display(Name = "職務")]
        Position,
        [Display(Name = "性別")]
        Gender
    }
}
