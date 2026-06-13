import { Component, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-wrapper">
      <!-- Sidebar -->
      <nav class="sidebar">
        <div class="sidebar-brand">
          <div class="brand-icon"><i class="bi bi-building-check"></i></div>
          <div>
            <div class="brand-name">EMS</div>
            <div class="brand-sub">Employee Management</div>
          </div>
        </div>

        <div class="nav-section">
          <div class="nav-label">Overview</div>
          <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
            <i class="bi bi-grid-1x2"></i> Dashboard
          </a>
        </div>

        <div class="nav-section">
          <div class="nav-label">People</div>
          <a routerLink="/employees" routerLinkActive="active" class="nav-item">
            <i class="bi bi-people"></i> Employees
          </a>
          @if (isAdminOrHR()) {
            <a routerLink="/departments" routerLinkActive="active" class="nav-item">
              <i class="bi bi-diagram-3"></i> Departments
            </a>
          }
        </div>

        <div class="nav-section">
          <div class="nav-label">Time Off</div>
          <a routerLink="/leave" routerLinkActive="active" class="nav-item">
            <i class="bi bi-calendar-check"></i> Leave Requests
          </a>
          <a routerLink="/leave/apply" routerLinkActive="active" class="nav-item">
            <i class="bi bi-plus-circle"></i> Apply for Leave
          </a>
        </div>

        <div class="sidebar-footer">
          <button class="logout-btn" (click)="auth.logout()">
            <i class="bi bi-box-arrow-left"></i> Sign Out
          </button>
        </div>
      </nav>

      <!-- Main content -->
      <div class="main-area">
        <header class="topbar">
          <div class="topbar-left">
            <span class="page-title">{{ pageTitle() }}</span>
          </div>
          <div class="topbar-right">
            <span class="role-badge role-{{ userRole().toLowerCase() }}">
              <i class="bi bi-shield-check"></i> {{ userRole() }}
            </span>
            <div class="user-info">
              <div class="avatar">{{ initials() }}</div>
              <span class="user-name">{{ userName() }}</span>
            </div>
          </div>
        </header>
        <main class="content">
          <router-outlet />
        </main>
      </div>
    </div>
  `
})
export class LayoutComponent {
  constructor(public auth: AuthService) {}

  userName   = computed(() => this.auth.currentUser()?.fullName ?? '');
  userRole   = computed(() => this.auth.currentUser()?.role ?? '');
  initials   = computed(() => this.userName().split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2));
  isAdminOrHR = computed(() => this.auth.hasRole('Admin', 'HR'));
  pageTitle  = computed(() => '');
}
