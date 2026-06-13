import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LeaveService } from '../../core/services/leave.service';

@Component({
  selector: 'app-leave-apply',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="d-flex align-items-center gap-3 mb-4">
      <a routerLink="/leave" class="btn btn-sm btn-outline-secondary"><i class="bi bi-arrow-left"></i></a>
      <h4 class="mb-0">Apply for Leave</h4>
    </div>

    @if (error()) { <div class="alert alert-danger">{{ error() }}</div> }
    @if (success()) { <div class="alert alert-success">Leave request submitted successfully!</div> }

    <div class="card-panel" style="max-width:560px">
      <form (ngSubmit)="submit()" #f="ngForm">
        <div class="mb-3">
          <label class="form-label">Leave Type *</label>
          <select class="form-select" [(ngModel)]="model.leaveType" name="leaveType" required>
            <option [value]="0">Annual</option>
            <option [value]="1">Sick</option>
            <option [value]="2">Maternity</option>
            <option [value]="3">Paternity</option>
            <option [value]="4">Unpaid</option>
            <option [value]="5">Compassionate</option>
          </select>
        </div>
        <div class="row g-3 mb-3">
          <div class="col-6">
            <label class="form-label">Start Date *</label>
            <input type="date" class="form-control" [(ngModel)]="model.startDate" name="startDate" required />
          </div>
          <div class="col-6">
            <label class="form-label">End Date *</label>
            <input type="date" class="form-control" [(ngModel)]="model.endDate" name="endDate" required />
          </div>
        </div>
        <div class="mb-3">
          <label class="form-label">Reason</label>
          <textarea class="form-control" [(ngModel)]="model.reason" name="reason" rows="3" placeholder="Brief reason for leave…"></textarea>
        </div>
        <button type="submit" class="btn btn-primary" [disabled]="loading()">
          @if (loading()) { <span class="spinner-border spinner-border-sm me-1"></span> }
          Submit Request
        </button>
      </form>
    </div>
  `
})
export class LeaveApplyComponent {
  model: any = { leaveType: 0 };
  error = signal(''); success = signal(false); loading = signal(false);

  constructor(private svc: LeaveService, private router: Router) {}

  submit() {
    this.loading.set(true);
    this.svc.apply(this.model).subscribe({
      next: () => { this.success.set(true); setTimeout(() => this.router.navigate(['/leave']), 1500); },
      error: (err) => { this.loading.set(false); this.error.set(err.error?.message ?? 'Failed to submit.'); }
    });
  }
}
