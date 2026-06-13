export interface Employee {
  id: number;
  employeeCode: string;
  fullName: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  gender: string;
  dateOfBirth: string;
  age: number;
  joinDate: string;
  jobTitle: string;
  department: string;
  departmentId: number;
  baseSalary: number;
  status: string;
  city: string;
  address?: string;
  postalCode?: string;
  emergencyContact?: string;
  reportsToName?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
  location?: string;
  managerName?: string;
  employeeCount: number;
}
