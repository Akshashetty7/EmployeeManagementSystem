import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EmployeeService } from '../../core/services/employee.service';
import { Department } from '../../core/models/employee.model';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="d-flex align-items-center gap-3 mb-4">
      <a routerLink="/employees" class="btn btn-sm btn-outline-secondary"><i class="bi bi-arrow-left"></i></a>
      <h4 class="mb-0">{{ isEdit ? 'Edit Employee' : 'Add New Employee' }}</h4>
    </div>

    @if (error()) {
      <div class="alert alert-danger">{{ error() }}</div>
    }

    <div class="card-panel">
      <form (ngSubmit)="save()" #f="ngForm">
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label">Employee Code *</label>
            <input type="text" class="form-control" [(ngModel)]="model.employeeCode" name="employeeCode" required [disabled]="isEdit" />
          </div>
          <div class="col-md-3">
            <label class="form-label">First Name *</label>
            <input type="text" class="form-control" [(ngModel)]="model.firstName" name="firstName" required />
          </div>
          <div class="col-md-3">
            <label class="form-label">Last Name *</label>
            <input type="text" class="form-control" [(ngModel)]="model.lastName" name="lastName" required />
          </div>
          <div class="col-md-3">
            <label class="form-label">Work Email *</label>
            <input type="email" class="form-control" [(ngModel)]="model.email" name="email" required />
          </div>
          <div class="col-md-3">
            <label class="form-label">Phone</label>
            <input type="text" class="form-control" [(ngModel)]="model.phone" name="phone" />
          </div>
          <div class="col-md-3">
            <label class="form-label">Gender *</label>
            <select class="form-select" [(ngModel)]="model.gender" name="gender" required>
              <option [value]="0">Male</option>
              <option [value]="1">Female</option>
              <option [value]="2">Other</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Date of Birth *</label>
            <input type="date" class="form-control" [(ngModel)]="model.dateOfBirth" name="dateOfBirth" required />
          </div>
          <div class="col-md-3">
            <label class="form-label">Join Date *</label>
            <input type="date" class="form-control" [(ngModel)]="model.joinDate" name="joinDate" required />
          </div>
          <div class="col-md-4">
            <label class="form-label">Job Title *</label>
            <input type="text" class="form-control" [(ngModel)]="model.jobTitle" name="jobTitle" required />
          </div>
          <div class="col-md-4">
            <label class="form-label">Department *</label>
            <select class="form-select" [(ngModel)]="model.departmentId" name="departmentId" required>
              <option [value]="0">Select…</option>
              @for (d of departments(); track d.id) {
                <option [value]="d.id">{{ d.name }}</option>
              }
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Base Salary *</label>
            <input type="number" class="form-control" [(ngModel)]="model.baseSalary" name="baseSalary" required />
          </div>
          <div class="col-md-4">
            <label class="form-label">City</label>
            <input type="text" class="form-control" [(ngModel)]="model.city" name="city" />
          </div>
          <div class="col-md-4">
            <label class="form-label">Emergency Contact</label>
            <input type="text" class="form-control" [(ngModel)]="model.emergencyContact" name="emergencyContact" />
          </div>
          @if (isEdit) {
            <div class="col-md-4">
              <label class="form-label">Status</label>
              <select class="form-select" [(ngModel)]="model.status" name="status">
                <option value="0">Active</option>
                <option value="1">OnLeave</option>
                <option value="2">Resigned</option>
                <option value="3">Terminated</option>
              </select>
            </div>
          }
        </div>

        <div class="mt-4">
          <button type="submit" class="btn btn-primary me-2" [disabled]="loading()">
            @if (loading()) { <span class="spinner-border spinner-border-sm me-1"></span> }
            {{ isEdit ? 'Save Changes' : 'Create Employee' }}
          </button>
          <a routerLink="/employees" class="btn btn-outline-secondary">Cancel</a>
        </div>
      </form>
    </div>
  `
})
export class EmployeeFormComponent implements OnInit {
  isEdit = false;
  model: any = { gender: 0, departmentId: 0, status: 0 };
  departments = signal<Department[]>([]);
  error = signal('');
  loading = signal(false);

  constructor(private route: ActivatedRoute, private router: Router, private svc: EmployeeService) {}

  ngOnInit() {
    this.svc.getDepartments().subscribe(d => this.departments.set(d));
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.svc.getById(+id).subscribe(emp => {
        this.model = {
          ...emp,
          dateOfBirth: emp.dateOfBirth.slice(0, 10),
          joinDate: emp.joinDate.slice(0, 10),
          gender: ['Male','Female','Other'].indexOf(emp.gender),
          status: ['Active','OnLeave','Resigned','Terminated'].indexOf(emp.status)
        };
      });
    }
  }

  save() {
    this.loading.set(true);
    const obs = this.isEdit
      ? this.svc.update(this.model.id, this.model)
      : this.svc.create(this.model);

    obs.subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err) => { this.loading.set(false); this.error.set(err.error?.message ?? 'Save failed.'); }
    });
  }
}
