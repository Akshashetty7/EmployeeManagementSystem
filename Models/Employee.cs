using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public enum EmploymentStatus { Active, OnLeave, Resigned, Terminated }
    public enum Gender { Male, Female, Other }

    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(10)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);

        [Required]
        [Display(Name = "Join Date")]
        public DateTime JoinDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Base Salary")]
        public decimal BaseSalary { get; set; }

        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [StringLength(6)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        public int? ReportsToId { get; set; }
        public virtual Employee? ReportsTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    }
}
