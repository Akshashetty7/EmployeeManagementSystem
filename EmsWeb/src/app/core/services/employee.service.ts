import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Employee, PagedResult, Department } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private base = `${environment.apiUrl}/employees`;
  private deptBase = `${environment.apiUrl}/departments`;

  constructor(private http: HttpClient) {}

  getAll(search = '', departmentId = 0, status = '', page = 1, pageSize = 10) {
    let params = new HttpParams()
      .set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    if (departmentId) params = params.set('departmentId', departmentId);
    if (status) params = params.set('status', status);
    return this.http.get<PagedResult<Employee>>(this.base, { params });
  }

  getById(id: number) {
    return this.http.get<Employee>(`${this.base}/${id}`);
  }

  create(emp: any) {
    return this.http.post(this.base, emp);
  }

  update(id: number, emp: any) {
    return this.http.put(`${this.base}/${id}`, emp);
  }

  terminate(id: number) {
    return this.http.delete(`${this.base}/${id}`);
  }

  getDepartments() {
    return this.http.get<Department[]>(this.deptBase);
  }
}
