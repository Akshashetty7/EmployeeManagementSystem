using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public enum LeaveType { Annual, Sick, Maternity, Paternity, Unpaid, Compassionate }
    public enum LeaveStatus { Pending, ApprovedByManager, RejectedByManager, ApprovedByHR, RejectedByHR, Cancelled }

    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public LeaveType LeaveType { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [NotMapped]
        public int TotalDays => (int)(EndDate - StartDate).TotalDays + 1;

        [Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        [StringLength(500)]
        [Display(Name = "Manager Remarks")]
        public string ManagerRemarks { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "HR Remarks")]
        public string HRRemarks { get; set; } = string.Empty;

        public string? ReviewedByManagerId { get; set; }
        public string? ReviewedByHRId { get; set; }

        public DateTime? ManagerReviewDate { get; set; }
        public DateTime? HRReviewDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
