using EmsApi.Models;

namespace EmsApi.DTOs;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagerRemarks { get; set; }
    public string? HRRemarks { get; set; }
    public DateTime AppliedOn { get; set; }
}

public class ApplyLeaveDto
{
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public int? EmployeeId { get; set; }
}

public class ReviewLeaveDto
{
    public string Action { get; set; } = string.Empty; // "approve" or "reject"
    public string? Remarks { get; set; }
}
