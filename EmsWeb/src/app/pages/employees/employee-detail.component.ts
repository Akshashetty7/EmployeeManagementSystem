import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EmployeeService } from '../../core/services/employee.service';
import { Employee } from '../../core/models/employee.model';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (emp()) {
      <div class="d-flex align-items-center gap-3 mb-4">
        <a routerLink="/employees" class="btn btn-sm btn-outline-secondary"><i class="bi bi-arrow-left"></i></a>
        <div>
          <h4 class="mb-0">{{ emp()!.fullName }}</h4>
          <span class="text-muted">{{ emp()!.jobTitle }} · {{ emp()!.department }}</span>
        </div>
        <span class="ms-auto status-badge status-{{ emp()!.status.toLowerCase() }}">{{ emp()!.status }}</span>
      </div>

      <div class="row g-3">
        <div class="col-md-6">
          <div class="card-panel">
            <h6 class="panel-title">Personal Info</h6>
            <table class="info-table">
              <tr><td>Employee Code</td><td><code>{{ emp()!.employeeCode }}</code></td></tr>
              <tr><td>Email</td><td>{{ emp()!.email }}</td></tr>
              <tr><td>Phone</td><td>{{ emp()!.phone ?? '—' }}</td></tr>
              <tr><td>Gender</td><td>{{ emp()!.gender }}</td></tr>
              <tr><td>Date of Birth</td><td>{{ emp()!.dateOfBirth | date:'dd MMM yyyy' }}</td></tr>
              <tr><td>Age</td><td>{{ emp()!.age }} years</td></tr>
              <tr><td>City</td><td>{{ emp()!.city }}</td></tr>
            </table>
          </div>
        </div>
        <div class="col-md-6">
          <div class="card-panel">
            <h6 class="panel-title">Employment Details</h6>
            <table class="info-table">
              <tr><td>Department</td><td>{{ emp()!.department }}</td></tr>
              <tr><td>Reports To</td><td>{{ emp()!.reportsToName ?? '—' }}</td></tr>
              <tr><td>Join Date</td><td>{{ emp()!.joinDate | date:'dd MMM yyyy' }}</td></tr>
              <tr><td>Base Salary</td><td>₹{{ emp()!.baseSalary | number }}</td></tr>
              <tr><td>Emergency Contact</td><td>{{ emp()!.emergencyContact ?? '—' }}</td></tr>
            </table>
          </div>
        </div>
      </div>
    } @else {
      <div class="text-center py-5"><div class="spinner-border text-primary"></div></div>
    }
  `
})
export class EmployeeDetailComponent implements OnInit {
  emp = signal<Employee | null>(null);

  constructor(private route: ActivatedRoute, private svc: EmployeeService) {}

  ngOnInit() {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.svc.getById(id).subscribe(e => this.emp.set(e));
  }
}
