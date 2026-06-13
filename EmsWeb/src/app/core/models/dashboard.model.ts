export interface Dashboard {
  totalEmployees: number;
  activeEmployees: number;
  onLeave: number;
  totalDepartments: number;
  pendingLeaves: number;
  newHiresThisMonth: number;
  totalPayroll: number;
  departmentStats: DeptStat[];
  leaveTypeStats: LeaveTypeStat[];
  recentHires: RecentHire[];
  monthlyHeadcount: MonthlyCount[];
}

export interface DeptStat { department: string; count: number; payroll: number; }
export interface LeaveTypeStat { leaveType: string; count: number; }
export interface RecentHire { id: number; fullName: string; jobTitle: string; department: string; joinDate: string; }
export interface MonthlyCount { month: string; count: number; }
