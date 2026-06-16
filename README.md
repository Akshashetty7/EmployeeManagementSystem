# Employee Management System (EMS)

A full-stack web application to manage employee records, departments, and leave requests — built with ASP.NET Core 8 Web API and Angular 21.

## Key Features

- JWT authentication with refresh tokens and account lockout
- Role-based access: Admin, HR, Manager, Employee
- Employee CRUD with pagination, search, and filtering
- Leave request workflow with manager and HR approval
- Bulk CSV import with per-row error handling
- Excel export via ClosedXML
- PII encryption for sensitive fields (NationalId)
- Optimistic concurrency with RowVersion conflict detection
- Full audit trail on every write operation
- Soft delete — terminated employees are never hard-deleted
