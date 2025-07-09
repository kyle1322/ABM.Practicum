using System;

namespace Timesheets_APP.ViewModels
{
    public class TimesheetViewModel
    {
        public int TsId { get; set; }
        public int EmpId { get; set; }              // new
        public string EmpName { get; set; } = "";
        public DateTime TsDate { get; set; }             // renamed from Date
        public DateTime Date { get; set; }   // ← this

        public bool Approved { get; set; }             // new
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public byte Hours { get; set; }
        public byte Minutes { get; set; }
        public bool Overtime { get; set; }             // already there
        public DateTime Modified { get; set; }             // new
        public List<TimesheetsItemViewModel> Items { get; set; } = new();
    }
}
