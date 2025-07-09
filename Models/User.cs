using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheets_APP.Models
{
    [Table("users")]
    public class User
    {
        [Key] 
        [Column("login_id")]
        [StringLength(64)]
        public string LoginId { get; set; } = null!;

        [Column("password")]
        [StringLength(128)]
        public string? Password { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("modified")]
        public DateTime Modified { get; set; }
    }
}
