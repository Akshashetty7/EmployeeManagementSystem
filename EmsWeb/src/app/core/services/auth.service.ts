import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, CurrentUser } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY         = 'ems_token';
  private readonly REFRESH_TOKEN_KEY = 'ems_refresh_token';
  private readonly USER_KEY          = 'ems_user';

  currentUser = signal<CurrentUser | null>(this.loadUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials).pipe(
      tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token);
        localStorage.setItem(this.REFRESH_TOKEN_KEY, res.refreshToken);
        const user: CurrentUser = {
          userId: res.userId,
          email: res.email,
          fullName: res.fullName,
          role: res.role
        };
        localStorage.setItem(this.USER_KEY, JSON.stringify(user));
        this.currentUser.set(user);
      })
    );
  }

  logout() {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      this.http.post(`${environment.apiUrl}/auth/logout`, { refreshToken }).subscribe();
    }
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  refreshAccessToken() {
    const refreshToken = this.getRefreshToken();
    return this.http.post<{ token: string; refreshToken: string }>(
      `${environment.apiUrl}/auth/refresh`,
      { refreshToken }
    ).pipe(
      tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token);
        localStorage.setItem(this.REFRESH_TOKEN_KEY, res.refreshToken);
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  hasRole(...roles: string[]): boolean {
    const user = this.currentUser();
    return !!user && roles.includes(user.role);
  }

  private loadUser(): CurrentUser | null {
    try {
      const raw = localStorage.getItem(this.USER_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch { return null; }
  }
}
