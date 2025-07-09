using System.ComponentModel.DataAnnotations;

namespace Timesheets_APP.ViewModels
{
    public class EmployeeLoginViewModel
    {
        [Required]
        [Display(Name = "Employee ID")]
        public int EmpId { get; set; }
    }
}
