using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Models;

namespace Timesheets_APP.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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

            builder.Entity<Timesheet>(e =>
            {
                e.HasKey(x => x.TsId);
                e.Property(x => x.EndTime).IsFixedLength();
                e.Property(x => x.Modified)
                    .HasDefaultValueSql("getdate()");
                e.Property(x => x.Overtime)
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();
                e.Property(x => x.StartTime).IsFixedLength();
                e.Property(x => x.TsApproved)
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();
                e.HasOne(x => x.Emp)
                    .WithMany(x => x.Timesheets)
                    .HasForeignKey(x => x.EmpId)
                    .HasConstraintName("fk_timesheets_employees");
            });

            builder.Entity<TimesheetsItem>(e =>
            {
                e.HasKey(x => x.TrId);
                e.Property(x => x.Modified)
                    .HasDefaultValueSql("getdate()");
                e.Property(x => x.TimeFrom).IsFixedLength();
                e.Property(x => x.TimeOut).IsFixedLength();
                e.HasOne(x => x.Ts)
                    .WithMany(x => x.TimesheetsItems)
                    .HasForeignKey(x => x.TsId)
                    .HasConstraintName("fk_items_timesheets");
            });
        }
    }
}
