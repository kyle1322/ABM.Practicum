using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Timesheets_APP.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("emp_id")]
        public int EmpId { get; set; }

        [Required]
        [StringLength(64)]
        [Column("login_id")]
        public string LoginId { get; set; } = null!;

        [Required]
        [StringLength(128)]
        [Column("emp_name")]
        public string EmpName { get; set; } = null!;

        [Required]
        [StringLength(1)]
        [Column("overtime", TypeName = "char(1)")]
        public string Overtime { get; set; } = "N";

        [Required]
        [Column("emp_start", TypeName = "date")]
        public DateTime EmpStart { get; set; }

        [Column("emp_end", TypeName = "date")]
        public DateTime? EmpEnd { get; set; }

        [Required]
        [Column("modified", TypeName = "datetime")]
        public DateTime Modified { get; set; }

        // navigation to timesheets
        public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
    }
}
