export interface LeaveRequest {
  id: number;
  employeeId: number;
  employeeName: string;
  department: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  totalDays: number;
  reason?: string;
  status: string;
  managerRemarks?: string;
  hrRemarks?: string;
  appliedOn: string;
}

export interface ApplyLeaveDto {
  leaveType: number;
  startDate: string;
  endDate: string;
  reason?: string;
}
