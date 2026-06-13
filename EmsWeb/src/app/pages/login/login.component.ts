import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="login-wrapper">
      <div class="left-panel">
        <div class="blob blob-1"></div>
        <div class="blob blob-2"></div>
        <div class="left-inner">
          <div class="brand">
            <div class="brand-icon"><i class="bi bi-building-check"></i></div>
            <span class="brand-name">EMS</span>
          </div>
          <div class="hero">
            <h1 class="hero-title">Your workforce,<br /><em>fully in control.</em></h1>
            <p class="hero-sub">Manage employees, track leave, handle approvals, and keep every record audit-ready — from one secure place.</p>
          </div>
          <p class="left-footer">&copy; 2025 Employee Management System</p>
        </div>
      </div>

      <div class="right-panel">
        <div class="form-box">
          <div class="form-head">
            <h2>Welcome back</h2>
            <p>Sign in with your work credentials</p>
          </div>

          @if (error()) {
            <div class="error-msg">
              <i class="bi bi-exclamation-circle-fill"></i> {{ error() }}
            </div>
          }

          <form (ngSubmit)="onSubmit()" #f="ngForm">
            <div class="field">
              <label>Work Email</label>
              <div class="input-wrap">
                <i class="bi bi-envelope icon"></i>
                <input type="email" [(ngModel)]="email" name="email" placeholder="you@company.com" required autocomplete="username" />
              </div>
            </div>

            <div class="field">
              <label>Password</label>
              <div class="input-wrap">
                <i class="bi bi-lock icon"></i>
                <input [type]="showPwd() ? 'text' : 'password'" [(ngModel)]="password" name="password" placeholder="••••••••" required autocomplete="current-password" />
                <button type="button" class="toggle-eye" (click)="showPwd.set(!showPwd())">
                  <i class="bi" [class.bi-eye]="!showPwd()" [class.bi-eye-slash]="showPwd()"></i>
                </button>
              </div>
            </div>

            <button type="submit" class="btn-sign" [disabled]="loading()">
              @if (loading()) { <span class="spinner-border spinner-border-sm me-2"></span> }
              @else { <i class="bi bi-box-arrow-in-right me-2"></i> }
              Sign In
            </button>
          </form>

          <div class="form-foot">Don't have access?<br />Contact your system administrator.</div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  error = signal('');
  loading = signal(false);
  showPwd = signal(false);

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    this.error.set('');
    this.loading.set(true);
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message ?? 'Login failed. Please try again.');
      }
    });
  }
}
