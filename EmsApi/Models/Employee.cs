using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmsApi.Models;

public enum EmploymentStatus { Active, OnLeave, Resigned, Terminated }
public enum Gender { Male, Female, Other }

public class Employee
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }

    [Required]
    public string JobTitle { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }

    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContact { get; set; }
    public string? NationalId { get; set; }        // encrypted via DataProtection
    public string? NationalIdHash { get; set; }    // SHA-256 hash for fast duplicate detection
    public int? ReportsToId { get; set; }

    // Problem 18: EF Core concurrency token — SQL Server auto-increments this on every UPDATE.
    // If two users fetch the same row and both try to save, the second save detects the mismatch
    // and throws DbUpdateConcurrencyException instead of silently overwriting.
    [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(
        System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
    public byte[]? RowVersion { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public int Age => DateTime.Today.Year - DateOfBirth.Year -
        (DateTime.Today < DateOfBirth.AddYears(DateTime.Today.Year - DateOfBirth.Year) ? 1 : 0);

    public virtual Department? Department { get; set; }
    public virtual Employee? ReportsTo { get; set; }
    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
