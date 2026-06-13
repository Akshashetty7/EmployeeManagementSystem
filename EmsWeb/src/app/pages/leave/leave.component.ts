import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LeaveService } from '../../core/services/leave.service';
import { AuthService } from '../../core/services/auth.service';
import { LeaveRequest } from '../../core/models/leave.model';

@Component({
  selector: 'app-leave',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page-header mb-4">
      <div>
        <h4 class="page-title-text">Leave Requests</h4>
        <p class="page-sub">{{ leaves().length }} total requests</p>
      </div>
      <a routerLink="/leave/apply" class="btn btn-primary btn-sm">
        <i class="bi bi-plus-lg me-1"></i> Apply for Leave
      </a>
    </div>

    <div class="table-card">
      <table class="table table-hover mb-0">
        <thead>
          <tr><th>Employee</th><th>Type</th><th>From</th><th>To</th><th>Days</th><th>Status</th><th>Applied</th><th>Actions</th></tr>
        </thead>
        <tbody>
          @for (l of leaves(); track l.id) {
            <tr>
              <td>
                <div class="fw-semibold">{{ l.employeeName }}</div>
                <div class="text-muted small">{{ l.department }}</div>
              </td>
              <td><span class="badge bg-secondary">{{ l.leaveType }}</span></td>
              <td>{{ l.startDate | date:'dd MMM' }}</td>
              <td>{{ l.endDate | date:'dd MMM yyyy' }}</td>
              <td>{{ l.totalDays }}</td>
              <td><span class="status-badge leave-{{ l.status.toLowerCase() }}">{{ l.status }}</span></td>
              <td>{{ l.appliedOn | date:'dd MMM yyyy' }}</td>
              <td>
                @if (canReview() && l.status === 'Pending') {
                  <button class="btn btn-xs btn-success me-1" (click)="review(l, 'approve')">
                    <i class="bi bi-check"></i>
                  </button>
                  <button class="btn btn-xs btn-danger" (click)="review(l, 'reject')">
                    <i class="bi bi-x"></i>
                  </button>
                }
                @if (l.status === 'Pending') {
                  <button class="btn btn-xs btn-outline-secondary ms-1" (click)="cancel(l)">
                    Cancel
                  </button>
                }
              </td>
            </tr>
          } @empty {
            <tr><td colspan="8" class="text-center text-muted py-4">No leave requests found</td></tr>
          }
        </tbody>
      </table>
    </div>
  `
})
export class LeaveComponent implements OnInit {
  leaves = signal<LeaveRequest[]>([]);

  constructor(private svc: LeaveService, public auth: AuthService) {}

  ngOnInit() { this.load(); }

  load() { this.svc.getAll().subscribe(l => this.leaves.set(l)); }

  canReview() { return this.auth.hasRole('Admin', 'HR', 'Manager'); }

  review(l: LeaveRequest, action: string) {
    const remarks = prompt(`${action === 'approve' ? 'Approval' : 'Rejection'} remarks (optional):`);
    this.svc.review(l.id, action, remarks ?? '').subscribe(() => this.load());
  }

  cancel(l: LeaveRequest) {
    if (confirm('Cancel this leave request?'))
      this.svc.cancel(l.id).subscribe(() => this.load());
  }
}
