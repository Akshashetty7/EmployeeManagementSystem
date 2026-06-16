import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

let isRefreshing = false;
const refreshDone$ = new BehaviorSubject<string | null>(null);

function addToken(req: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

export const jwtInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  if (token) {
    req = addToken(req, token);
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Only handle 401 — and only if we have a refresh token to try
      if (error.status !== 401 || !auth.getRefreshToken()) {
        return throwError(() => error);
      }

      if (isRefreshing) {
        // Another request is already refreshing — wait for it to finish, then retry
        return refreshDone$.pipe(
          filter(t => t !== null),
          take(1),
          switchMap(newToken => next(addToken(req, newToken!)))
        );
      }

      // Start refresh
      isRefreshing = true;
      refreshDone$.next(null);

      return auth.refreshAccessToken().pipe(
        switchMap(res => {
          isRefreshing = false;
          refreshDone$.next(res.token);
          return next(addToken(req, res.token));
        }),
        catchError(refreshError => {
          // Refresh token itself expired or invalid → force logout
          isRefreshing = false;
          auth.logout();
          return throwError(() => refreshError);
        })
      );
    })
  );
};
