# Employee Management System (EMS)

A full-featured enterprise Employee Management System built with **ASP.NET Core 8 MVC**, ASP.NET Identity authentication, Entity Framework Core 8, and SQLite.

---

## What's Inside

| Module | Features |
|---|---|
| **Authentication** | Login, logout, account lockout, role-based access |
| **Employee Directory** | Add, edit, view, terminate, search, filter, paginate |
| **Leave Management** | Apply, multi-level approve/reject (Manager → HR), cancel |
| **Departments** | Create, manage, assign department heads |
| **Dashboard** | KPI cards, department chart, leave distribution chart |
| **Reports** | Export employee roster and leave report to Excel |
| **Audit Trail** | Every create/update/terminate action is logged with timestamp |

---

## Prerequisites

Install these before running the project:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | **8.x** | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Git | Any | https://git-scm.com/downloads |

> No SQL Server, no MySQL, no database setup needed. The app uses **SQLite** — the database file is created automatically the first time you run the project.

---

## Run on Any Laptop — Step by Step

### 1. Clone the repository

```bash
git clone https://github.com/Akshashetty7/EmployeeManagementSystem.git
cd EmployeeManagementSystem
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Run the project

```bash
dotnet run
```

That's it. On first run the app will:
- Create the SQLite database file (`ems.db`) automatically
- Apply all migrations
- Seed **25 sample employees**, 6 departments, 7 leave requests, and all demo user accounts

Open your browser at:
```
http://localhost:5000
```

---

## Database

**No SQL script required.** The database is managed entirely by EF Core migrations and is seeded on first startup.

The SQLite file (`ems.db`) is listed in `.gitignore` and is never committed — each machine gets a fresh database on first run.

If you ever need to reset the database (start fresh), just delete `ems.db` and re-run the project:

```bash
# Windows
del ems.db

# Mac / Linux
rm ems.db

dotnet run
```

---

## Login Credentials

The following accounts are seeded automatically:

| Role | Email | Password |
|---|---|---|
| **Admin** | admin@ems.com | Admin@123! |
| **HR** | anjali.verma@ems.com | Hr@123! |
| **Manager** | arjun.mehta@ems.com | Mgr@123! |
| **Employee** | sneha.kapoor@ems.com | Emp@123! |

### Role Permissions

| Action | Admin | HR | Manager | Employee |
|---|:---:|:---:|:---:|:---:|
| View all employees | ✅ | ✅ | ✅ | ✅ |
| Add / edit employees | ✅ | ✅ | ✅ (own team) | ❌ |
| Terminate employee | ✅ | ❌ | ❌ | ❌ |
| Manage departments | ✅ | ✅ | ❌ | ❌ |
| Approve leave (1st level) | ✅ | ✅ | ✅ | ❌ |
| Approve leave (final) | ✅ | ✅ | ❌ | ❌ |
| Export Excel reports | ✅ | ✅ | ❌ | ❌ |
| View audit logs | ✅ | ✅ | ✅ | ✅ |

---

## Sample Data

25 employees seeded across 6 departments:

| Department | Head | Employees |
|---|---|---|
| Engineering | Arjun Mehta (CTO) | 5 |
| Human Resources | Anjali Verma (HR Manager) | 3 |
| Finance | Suresh Pillai (CFO) | 4 |
| Marketing | Aditya Bose (Director) | 3 |
| Operations | Ravi Choudhary (Head) | 4 |
| Sales | Nikhil Desai (VP) | 6 |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| Authentication | ASP.NET Core Identity |
| ORM | Entity Framework Core 8 |
| Database | SQLite (file-based, zero setup) |
| UI | Bootstrap 5.3 + Bootstrap Icons |
| Charts | Chart.js 4 |
| Excel Export | ClosedXML |
| Language | C# 12 / .NET 8 |

---

## Project Structure

```
EmployeeManagementSystem/
├── Controllers/          # MVC controllers (Account, Dashboard, Employees, Leave, Departments)
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext
│   └── SeedData.cs               # Auto-seeds employees, users, leave requests on startup
├── Migrations/           # EF Core migrations (auto-applied on startup)
├── Models/               # Entity models + ViewModels
├── Services/
│   ├── AuditService.cs           # Logs every create/update/delete action
│   ├── ExportService.cs          # Excel export via ClosedXML
│   └── NotificationService.cs   # Email notification stub (logs to console)
├── Views/                # Razor views for all pages
├── Program.cs            # App startup, DI, middleware, security config
└── appsettings.json      # Connection string + logging config
```

---

## Security Features

- Passwords hashed with **PBKDF2 + salt** (ASP.NET Identity default — never stored in plaintext)
- Auth cookie: `HttpOnly`, `SameSite=Strict`
- Account lockout after **5 failed attempts** (10 minute lockout)
- **Anti-CSRF tokens** on all POST forms
- Security headers: `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`
- Database file excluded from git via `.gitignore` (no employee data in source control)

---

## Troubleshooting

**Port already in use**
```bash
dotnet run --urls http://localhost:5001
```

**Reset everything (fresh database + users)**
```bash
del ems.db        # Windows
dotnet run
```

**Check .NET version**
```bash
dotnet --version  # Must start with 8.
```
