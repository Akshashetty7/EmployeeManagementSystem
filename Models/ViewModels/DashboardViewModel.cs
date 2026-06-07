namespace EmployeeManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int NewHiresThisMonth { get; set; }
        public decimal TotalPayroll { get; set; }

        public List<DepartmentStat> DepartmentStats { get; set; } = new();
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public List<LeaveRequest> PendingLeaves { get; set; } = new();
        public List<Employee> RecentEmployees { get; set; } = new();
        public Dictionary<string, int> LeaveTypeDistribution { get; set; } = new();
        public Dictionary<string, int> MonthlyHeadcount { get; set; } = new();
    }

    public class DepartmentStat
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AverageSalary { get; set; }
    }

    public class RecentActivity
    {
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string BadgeClass { get; set; } = "bg-primary";
    }
}
