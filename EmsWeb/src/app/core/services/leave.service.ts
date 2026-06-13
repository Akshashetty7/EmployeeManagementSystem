import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LeaveRequest, ApplyLeaveDto } from '../models/leave.model';

@Injectable({ providedIn: 'root' })
export class LeaveService {
  private base = `${environment.apiUrl}/leave`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<LeaveRequest[]>(this.base);
  }

  apply(dto: ApplyLeaveDto) {
    return this.http.post(`${this.base}/apply`, dto);
  }

  review(id: number, action: string, remarks: string) {
    return this.http.post(`${this.base}/${id}/review`, { action, remarks });
  }

  cancel(id: number) {
    return this.http.post(`${this.base}/${id}/cancel`, {});
  }
}
