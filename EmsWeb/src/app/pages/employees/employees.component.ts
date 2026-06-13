import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../core/services/employee.service';
import { AuthService } from '../../core/services/auth.service';
import { Employee, Department, PagedResult } from '../../core/models/employee.model';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page-header mb-4">
      <div>
        <h4 class="page-title-text">Employees</h4>
        <p class="page-sub">{{ result()?.totalCount ?? 0 }} total records</p>
      </div>
      @if (auth.hasRole('Admin', 'HR')) {
        <a routerLink="/employees/create" class="btn btn-primary btn-sm">
          <i class="bi bi-plus-lg me-1"></i> Add Employee
        </a>
      }
    </div>

    <!-- Filters -->
    <div class="filter-bar mb-3">
      <input type="text" [(ngModel)]="search" (ngModelChange)="onSearch()" placeholder="Search name, email, code…" class="form-control form-control-sm" style="max-width:260px" />
      <select [(ngModel)]="deptFilter" (ngModelChange)="load()" class="form-select form-select-sm" style="max-width:180px">
        <option value="0">All Departments</option>
        @for (d of departments(); track d.id) {
          <option [value]="d.id">{{ d.name }}</option>
        }
      </select>
      <select [(ngModel)]="statusFilter" (ngModelChange)="load()" class="form-select form-select-sm" style="max-width:150px">
        <option value="">All Status</option>
        <option value="Active">Active</option>
        <option value="OnLeave">On Leave</option>
        <option value="Resigned">Resigned</option>
        <option value="Terminated">Terminated</option>
      </select>
    </div>

    <!-- Table -->
    <div class="table-card">
      <table class="table table-hover mb-0">
        <thead>
          <tr>
            <th>Code</th><th>Name</th><th>Title</th><th>Department</th><th>Status</th><th>City</th><th>Actions</th>
          </tr>
        </thead>
        <tbody>
          @for (emp of result()?.items; track emp.id) {
            <tr>
              <td><code>{{ emp.employeeCode }}</code></td>
              <td>
                <a [routerLink]="['/employees', emp.id]" class="fw-semibold">{{ emp.fullName }}</a>
                <div class="text-muted small">{{ emp.email }}</div>
              </td>
              <td>{{ emp.jobTitle }}</td>
              <td>{{ emp.department }}</td>
              <td><span class="status-badge status-{{ emp.status.toLowerCase() }}">{{ emp.status }}</span></td>
              <td>{{ emp.city }}</td>
              <td>
                <a [routerLink]="['/employees', emp.id]" class="btn btn-xs btn-outline-secondary me-1" title="View"><i class="bi bi-eye"></i></a>
                @if (auth.hasRole('Admin', 'HR', 'Manager')) {
                  <a [routerLink]="['/employees', emp.id, 'edit']" class="btn btn-xs btn-outline-primary me-1" title="Edit"><i class="bi bi-pencil"></i></a>
                }
                @if (auth.hasRole('Admin') && emp.status !== 'Terminated') {
                  <button class="btn btn-xs btn-outline-danger" (click)="terminate(emp)" title="Terminate"><i class="bi bi-x-circle"></i></button>
                }
              </td>
            </tr>
          } @empty {
            <tr><td colspan="7" class="text-center text-muted py-4">No employees found</td></tr>
          }
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    @if (result() && result()!.totalPages > 1) {
      <div class="pagination-bar mt-3">
        <button class="btn btn-sm btn-outline-secondary" [disabled]="page === 1" (click)="goPage(page - 1)">
          <i class="bi bi-chevron-left"></i>
        </button>
        <span class="px-3">Page {{ page }} of {{ result()!.totalPages }}</span>
        <button class="btn btn-sm btn-outline-secondary" [disabled]="page >= result()!.totalPages" (click)="goPage(page + 1)">
          <i class="bi bi-chevron-right"></i>
        </button>
      </div>
    }
  `
})
export class EmployeesComponent implements OnInit {
  result = signal<PagedResult<Employee> | null>(null);
  departments = signal<Department[]>([]);
  search = ''; deptFilter = 0; statusFilter = ''; page = 1;
  private searchTimer: any;

  constructor(private svc: EmployeeService, public auth: AuthService) {}

  ngOnInit() {
    this.load();
    this.svc.getDepartments().subscribe(d => this.departments.set(d));
  }

  load() {
    this.svc.getAll(this.search, this.deptFilter, this.statusFilter, this.page)
      .subscribe(r => this.result.set(r));
  }

  onSearch() {
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => { this.page = 1; this.load(); }, 400);
  }

  goPage(p: number) { this.page = p; this.load(); }

  terminate(emp: Employee) {
    if (confirm(`Terminate ${emp.fullName}? This cannot be undone.`)) {
      this.svc.terminate(emp.id).subscribe(() => this.load());
    }
  }
}
