namespace EmsApi.DTOs;

public class DashboardDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int OnLeave { get; set; }
    public int TotalDepartments { get; set; }
    public int PendingLeaves { get; set; }
    public int NewHiresThisMonth { get; set; }
    public decimal TotalPayroll { get; set; }
    public List<DeptStatDto> DepartmentStats { get; set; } = new();
    public List<LeaveTypeStatDto> LeaveTypeStats { get; set; } = new();
    public List<RecentHireDto> RecentHires { get; set; } = new();
    public List<MonthlyCountDto> MonthlyHeadcount { get; set; } = new();
}

public record DeptStatDto(string Department, int Count, decimal Payroll);
public record LeaveTypeStatDto(string LeaveType, int Count);
public record RecentHireDto(int Id, string FullName, string JobTitle, string Department, DateTime JoinDate);
public record MonthlyCountDto(string Month, int Count);
