import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LayoutComponent } from './shared/layout/layout.component';
import { LoginComponent } from './pages/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { EmployeesComponent } from './pages/employees/employees.component';
import { EmployeeDetailComponent } from './pages/employees/employee-detail.component';
import { EmployeeFormComponent } from './pages/employees/employee-form.component';
import { LeaveComponent } from './pages/leave/leave.component';
import { LeaveApplyComponent } from './pages/leave/leave-apply.component';
import { DepartmentsComponent } from './pages/departments/departments.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '',               redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',      component: DashboardComponent },
      { path: 'employees',          component: EmployeesComponent },
      { path: 'employees/create',   component: EmployeeFormComponent },
      { path: 'employees/:id',      component: EmployeeDetailComponent },
      { path: 'employees/:id/edit', component: EmployeeFormComponent },
      { path: 'leave',              component: LeaveComponent },
      { path: 'leave/apply',        component: LeaveApplyComponent },
      { path: 'departments',        component: DepartmentsComponent },
    ]
  },
  { path: '**', redirectTo: '' }
];
