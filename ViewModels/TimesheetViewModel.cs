using System;

namespace Timesheets_APP.ViewModels
{
    public class TimesheetViewModel
    {
        public int TsId { get; set; }
        public int EmpId { get; set; }             
        public string EmpName { get; set; } = "";
        public DateTime TsDate { get; set; }            
        public DateTime Date { get; set; }  

        public bool Approved { get; set; }            
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public byte Hours { get; set; }
        public byte Minutes { get; set; }
        public bool Overtime { get; set; }            
        public DateTime Modified { get; set; }            
        public List<TimesheetsItemViewModel> Items { get; set; } = new();
    }
}
