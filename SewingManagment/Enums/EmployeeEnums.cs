using System.ComponentModel.DataAnnotations;

namespace SewingManagment.Enums
{
    public class EmployeeEnums
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

        public enum EmployeeSortField
        {
            [Display(Name = "工號")]
            Id,
            [Display(Name = "姓名")]
            Name,
            [Display(Name = "職務")]
            Position,
            [Display(Name = "性別")]
            Gender
        }
    }

}
