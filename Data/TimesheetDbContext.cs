using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Models;

namespace Timesheets_APP.Data
{
    public class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetsItem> TimesheetsItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Employee>(e =>
            {
                e.HasKey(x => x.EmpId);
                e.Property(x => x.Modified)
                    .HasDefaultValueSql("getdate()");
                e.Property(x => x.Overtime)
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();
            });
        }
    }
}
