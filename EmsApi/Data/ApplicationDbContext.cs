using EmsApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmsApi.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.EmployeeCode).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();

            // Problem 19: NationalId is now encrypted — raw values are non-deterministic ciphertext,
            // so we index the SHA-256 hash column instead for reliable duplicate detection.
            e.HasIndex(x => x.NationalIdHash).IsUnique().HasFilter("[NationalIdHash] IS NOT NULL");

            // Problem 18: RowVersion is the concurrency token.
            // EF Core adds it to every UPDATE WHERE clause automatically.
            e.Property(x => x.RowVersion).IsRowVersion();

            e.HasOne(x => x.Department)
             .WithMany(d => d.Employees)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ReportsTo)
             .WithMany(x => x.Subordinates)
             .HasForeignKey(x => x.ReportsToId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AuditLog>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.AuditLogs)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
