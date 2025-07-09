using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Timesheets_APP.Models
{
    [Table("timesheets")]
    public partial class Timesheet
    {
        public Timesheet()
        {
            TimesheetsItems = new HashSet<TimesheetsItem>();
        }

        [Key]
        [Column("ts_id")]
        public int TsId { get; set; }
        [Column("emp_id")]
        public int EmpId { get; set; }
        [Column("ts_date", TypeName = "date")]
        public DateTime TsDate { get; set; }
        [Column("ts_approved")]
        [StringLength(1)]
        [Unicode(false)]
        public string TsApproved { get; set; } = null!;
        [Column("start_time")]
        [StringLength(5)]
        [Unicode(false)]
        public string StartTime { get; set; } = null!;
        [Column("end_time")]
        [StringLength(5)]
        [Unicode(false)]
        public string EndTime { get; set; } = null!;
        [Column("hours")]
        public byte Hours { get; set; }
        [Column("minutes")]
        public byte Minutes { get; set; }
        [Column("overtime")]
        [StringLength(1)]
        [Unicode(false)]
        public string Overtime { get; set; } = null!;
        [Column("modified", TypeName = "datetime")]
        public DateTime Modified { get; set; }

        [ForeignKey("EmpId")]
        [InverseProperty("Timesheets")]
        public virtual Employee Emp { get; set; } = null!;
        [InverseProperty("Ts")]
        public virtual ICollection<TimesheetsItem> TimesheetsItems { get; set; }
    }
}
