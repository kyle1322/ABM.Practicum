using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Timesheets_APP.Models
{
    [Table("timesheets_items")]
    public partial class TimesheetsItem
    {
        [Key]
        [Column("tr_id")]
        public int TrId { get; set; }
        [Column("ts_id")]
        public int TsId { get; set; }
        [Column("time_from")]
        [StringLength(5)]
        [Unicode(false)]
        public string TimeFrom { get; set; } = null!;
        [Column("time_out")]
        [StringLength(5)]
        [Unicode(false)]
        public string TimeOut { get; set; } = null!;
        [Column("description")]
        [StringLength(128)]
        [Unicode(false)]
        public string? Description { get; set; }
        [Column("modified", TypeName = "datetime")]
        public DateTime? Modified { get; set; }

        [ForeignKey("TsId")]
        [InverseProperty("TimesheetsItems")]
        public virtual Timesheet Ts { get; set; } = null!;
    }
}
