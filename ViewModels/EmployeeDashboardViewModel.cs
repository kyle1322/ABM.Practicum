using System;
using System.Collections.Generic;
using Timesheets_APP.Models; 

namespace Timesheets_APP.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; } = "";
        public List<TimesheetViewModel> Timesheets { get; set; }
            = new List<TimesheetViewModel>();
    }
}
