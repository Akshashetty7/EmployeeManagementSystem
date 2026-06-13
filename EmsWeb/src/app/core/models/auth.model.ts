export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
}

export interface CurrentUser {
  userId: string;
  email: string;
  fullName: string;
  role: string;
  employeeId?: number;
}
