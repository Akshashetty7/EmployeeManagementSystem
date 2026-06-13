import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { Dashboard } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (data()) {
      <div class="page-header mb-4">
        <h4 class="page-title-text">Dashboard</h4>
        <p class="page-sub">Welcome back, {{ data()?.recentHires?.length ? 'here\'s your workforce overview' : '' }}</p>
      </div>

      <!-- KPI Cards -->
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="kpi-card">
            <div class="kpi-icon bg-blue"><i class="bi bi-people-fill"></i></div>
            <div>
              <div class="kpi-value">{{ data()!.totalEmployees }}</div>
              <div class="kpi-label">Total Employees</div>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="kpi-card">
            <div class="kpi-icon bg-green"><i class="bi bi-person-check-fill"></i></div>
            <div>
              <div class="kpi-value">{{ data()!.activeEmployees }}</div>
              <div class="kpi-label">Active</div>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="kpi-card">
            <div class="kpi-icon bg-orange"><i class="bi bi-calendar-x-fill"></i></div>
            <div>
              <div class="kpi-value">{{ data()!.pendingLeaves }}</div>
              <div class="kpi-label">Pending Leaves</div>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="kpi-card">
            <div class="kpi-icon bg-purple"><i class="bi bi-currency-rupee"></i></div>
            <div>
              <div class="kpi-value">₹{{ (data()!.totalPayroll / 100000).toFixed(1) }}L</div>
              <div class="kpi-label">Monthly Payroll</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Stats Tables -->
      <div class="row g-3 mb-4">
        <div class="col-md-7">
          <div class="card-panel">
            <h6 class="panel-title">Department Headcount</h6>
            @for (d of data()!.departmentStats; track d.department) {
              <div class="dept-row">
                <span class="dept-name">{{ d.department }}</span>
                <div class="dept-bar-wrap">
                  <div class="dept-bar" [style.width.%]="barWidth(d.count)"></div>
                </div>
                <span class="dept-count">{{ d.count }}</span>
              </div>
            }
          </div>
        </div>
        <div class="col-md-5">
          <div class="card-panel">
            <h6 class="panel-title">Leave Distribution</h6>
            @for (l of data()!.leaveTypeStats; track l.leaveType) {
              <div class="leave-row">
                <span class="leave-badge">{{ l.leaveType }}</span>
                <span class="leave-count">{{ l.count }}</span>
              </div>
            }
          </div>
        </div>
      </div>

      <!-- Recent Hires -->
      <div class="card-panel">
        <h6 class="panel-title">Recent Hires</h6>
        <table class="table table-sm table-hover mb-0">
          <thead><tr><th>Name</th><th>Title</th><th>Department</th><th>Joined</th></tr></thead>
          <tbody>
            @for (h of data()!.recentHires; track h.id) {
              <tr>
                <td><a [routerLink]="['/employees', h.id]">{{ h.fullName }}</a></td>
                <td>{{ h.jobTitle }}</td>
                <td>{{ h.department }}</td>
                <td>{{ h.joinDate | date:'dd MMM yyyy' }}</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    } @else {
      <div class="text-center py-5"><div class="spinner-border text-primary"></div></div>
    }
  `
})
export class DashboardComponent implements OnInit {
  data = signal<Dashboard | null>(null);

  constructor(private svc: DashboardService) {}

  ngOnInit() {
    this.svc.get().subscribe(d => this.data.set(d));
  }

  barWidth(count: number): number {
    const max = Math.max(...(this.data()?.departmentStats.map(d => d.count) ?? [1]));
    return Math.round((count / max) * 100);
  }
}
