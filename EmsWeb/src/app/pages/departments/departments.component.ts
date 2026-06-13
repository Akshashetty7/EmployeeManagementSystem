import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../core/services/employee.service';
import { Department } from '../../core/models/employee.model';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-header mb-4">
      <div>
        <h4 class="page-title-text">Departments</h4>
        <p class="page-sub">{{ departments().length }} active departments</p>
      </div>
    </div>

    <div class="row g-3">
      @for (d of departments(); track d.id) {
        <div class="col-md-4">
          <div class="card-panel dept-card">
            <div class="dept-icon"><i class="bi bi-building"></i></div>
            <h6 class="mt-3 mb-1">{{ d.name }}</h6>
            <p class="text-muted small mb-2">{{ d.description }}</p>
            <div class="d-flex gap-3 mt-2">
              <div class="text-center">
                <div class="fw-bold">{{ d.employeeCount }}</div>
                <div class="text-muted small">Employees</div>
              </div>
              <div class="text-center">
                <div class="fw-semibold small">{{ d.managerName ?? '—' }}</div>
                <div class="text-muted small">Manager</div>
              </div>
              @if (d.location) {
                <div class="text-center">
                  <div class="fw-semibold small">{{ d.location }}</div>
                  <div class="text-muted small">Location</div>
                </div>
              }
            </div>
          </div>
        </div>
      } @empty {
        <div class="text-center text-muted py-5">No departments found</div>
      }
    </div>
  `
})
export class DepartmentsComponent implements OnInit {
  departments = signal<Department[]>([]);

  constructor(private svc: EmployeeService) {}

  ngOnInit() { this.svc.getDepartments().subscribe(d => this.departments.set(d)); }
}
