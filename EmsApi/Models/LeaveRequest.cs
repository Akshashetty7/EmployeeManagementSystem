using System.ComponentModel.DataAnnotations.Schema;

namespace EmsApi.Models;

public enum LeaveType { Annual, Sick, Maternity, Paternity, Unpaid, Compassionate }
public enum LeaveStatus
{
    Pending,
    ApprovedByManager,
    RejectedByManager,
    ApprovedByHR,
    RejectedByHR,
    Cancelled
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [NotMapped]
    public int TotalDays => (EndDate - StartDate).Days + 1;

    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ManagerRemarks { get; set; }
    public string? HRRemarks { get; set; }
    public string? ReviewedByManagerId { get; set; }
    public string? ReviewedByHRId { get; set; }
    public DateTime? ManagerReviewDate { get; set; }
    public DateTime? HRReviewDate { get; set; }
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

    public virtual Employee? Employee { get; set; }
}
