using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Timesheets_APP.ViewModels
{
    public class AddTimesheetViewModel
    {
        [Required]
        public int EmpId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmpName { get; set; } = "";

        [Required]
        [Display(Name = "Timesheet Date")]
        public DateTime TsDate { get; set; }

        // Populated in the controller with the last 14 days
        public IEnumerable<SelectListItem> TsDateOptions { get; set; } = new List<SelectListItem>();

        [Required]
        [Display(Name = "Start Time")]
        public string StartTime { get; set; } = "09:00";

        [Required]
        [Display(Name = "End Time")]
        public string EndTime { get; set; } = "17:00";

        public List<TimesheetsItemViewModel> Items { get; set; } = new();
    }
}
