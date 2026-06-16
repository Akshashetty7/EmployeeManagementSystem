using EmsApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmsApi.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Roles
        foreach (var role in new[] { "Admin", "HR", "Manager", "Employee" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        if (await context.Departments.AnyAsync() && await context.Employees.AnyAsync()) return;

        // Departments
        var departments = new List<Department>
        {
            new() { Name = "Engineering",      Description = "Software development and architecture", Location = "Floor 3", IsActive = true },
            new() { Name = "Human Resources",  Description = "People operations and talent management", Location = "Floor 1", IsActive = true },
            new() { Name = "Finance",          Description = "Financial planning and accounting", Location = "Floor 2", IsActive = true },
            new() { Name = "Marketing",        Description = "Brand, campaigns and growth", Location = "Floor 2", IsActive = true },
            new() { Name = "Operations",       Description = "Infrastructure and process management", Location = "Floor 4", IsActive = true },
            new() { Name = "Sales",            Description = "Revenue and client relationships", Location = "Floor 1", IsActive = true },
        };
        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        int eng = departments[0].Id, hr = departments[1].Id, fin = departments[2].Id,
            mkt = departments[3].Id, ops = departments[4].Id, sal = departments[5].Id;

        // Employees
        var employees = new List<Employee>
        {
            new() { EmployeeCode="EMP001", FirstName="Arjun",    LastName="Mehta",      Email="arjun.mehta@ems.com",      Phone="9876543210", Gender=Gender.Male,   DateOfBirth=new DateTime(1985,3,15),  JoinDate=new DateTime(2018,1,10), JobTitle="Chief Technology Officer",  DepartmentId=eng, BaseSalary=180000, Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP002", FirstName="Anjali",   LastName="Verma",      Email="anjali.verma@ems.com",     Phone="9876543211", Gender=Gender.Female, DateOfBirth=new DateTime(1988,7,22),  JoinDate=new DateTime(2019,3,1),  JobTitle="HR Manager",               DepartmentId=hr,  BaseSalary=120000, Status=EmploymentStatus.Active, City="Mumbai" },
            new() { EmployeeCode="EMP003", FirstName="Suresh",   LastName="Pillai",     Email="suresh.pillai@ems.com",    Phone="9876543212", Gender=Gender.Male,   DateOfBirth=new DateTime(1980,11,5),  JoinDate=new DateTime(2017,6,15), JobTitle="Chief Financial Officer",  DepartmentId=fin, BaseSalary=190000, Status=EmploymentStatus.Active, City="Chennai" },
            new() { EmployeeCode="EMP004", FirstName="Aditya",   LastName="Bose",       Email="aditya.bose@ems.com",      Phone="9876543213", Gender=Gender.Male,   DateOfBirth=new DateTime(1987,4,18),  JoinDate=new DateTime(2020,2,1),  JobTitle="Marketing Director",       DepartmentId=mkt, BaseSalary=140000, Status=EmploymentStatus.Active, City="Delhi" },
            new() { EmployeeCode="EMP005", FirstName="Ravi",     LastName="Choudhary",  Email="ravi.choudhary@ems.com",   Phone="9876543214", Gender=Gender.Male,   DateOfBirth=new DateTime(1983,9,30),  JoinDate=new DateTime(2018,8,20), JobTitle="Head of Operations",       DepartmentId=ops, BaseSalary=150000, Status=EmploymentStatus.Active, City="Hyderabad" },
            new() { EmployeeCode="EMP006", FirstName="Nikhil",   LastName="Desai",      Email="nikhil.desai@ems.com",     Phone="9876543215", Gender=Gender.Male,   DateOfBirth=new DateTime(1986,1,12),  JoinDate=new DateTime(2019,5,10), JobTitle="VP Sales",                 DepartmentId=sal, BaseSalary=160000, Status=EmploymentStatus.Active, City="Pune" },
            new() { EmployeeCode="EMP007", FirstName="Sneha",    LastName="Kapoor",     Email="sneha.kapoor@ems.com",     Phone="9876543216", Gender=Gender.Female, DateOfBirth=new DateTime(1995,6,25),  JoinDate=new DateTime(2021,7,1),  JobTitle="Software Engineer",        DepartmentId=eng, BaseSalary=75000,  Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP008", FirstName="Rahul",    LastName="Sharma",     Email="rahul.sharma@ems.com",     Phone="9876543217", Gender=Gender.Male,   DateOfBirth=new DateTime(1993,2,14),  JoinDate=new DateTime(2020,11,15), JobTitle="Senior Developer",        DepartmentId=eng, BaseSalary=95000,  Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP009", FirstName="Priya",    LastName="Nair",       Email="priya.nair@ems.com",       Phone="9876543218", Gender=Gender.Female, DateOfBirth=new DateTime(1992,8,3),   JoinDate=new DateTime(2021,1,10), JobTitle="DevOps Engineer",          DepartmentId=eng, BaseSalary=88000,  Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP010", FirstName="Karan",    LastName="Singh",      Email="karan.singh@ems.com",      Phone="9876543219", Gender=Gender.Male,   DateOfBirth=new DateTime(1996,12,20), JoinDate=new DateTime(2022,3,1),  JobTitle="Junior Developer",        DepartmentId=eng, BaseSalary=55000,  Status=EmploymentStatus.Active, City="Noida" },
            new() { EmployeeCode="EMP011", FirstName="Meera",    LastName="Iyer",       Email="meera.iyer@ems.com",       Phone="9876543220", Gender=Gender.Female, DateOfBirth=new DateTime(1991,5,17),  JoinDate=new DateTime(2020,9,1),  JobTitle="HR Executive",            DepartmentId=hr,  BaseSalary=58000,  Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP012", FirstName="Vikram",   LastName="Rao",        Email="vikram.rao@ems.com",       Phone="9876543221", Gender=Gender.Male,   DateOfBirth=new DateTime(1990,7,8),   JoinDate=new DateTime(2021,4,15), JobTitle="Talent Acquisition Lead", DepartmentId=hr,  BaseSalary=72000,  Status=EmploymentStatus.Active, City="Mumbai" },
            new() { EmployeeCode="EMP013", FirstName="Deepa",    LastName="Krishnan",   Email="deepa.krishnan@ems.com",   Phone="9876543222", Gender=Gender.Female, DateOfBirth=new DateTime(1989,3,11),  JoinDate=new DateTime(2019,6,1),  JobTitle="Finance Manager",         DepartmentId=fin, BaseSalary=98000,  Status=EmploymentStatus.Active, City="Chennai" },
            new() { EmployeeCode="EMP014", FirstName="Amit",     LastName="Gupta",      Email="amit.gupta@ems.com",       Phone="9876543223", Gender=Gender.Male,   DateOfBirth=new DateTime(1994,10,29), JoinDate=new DateTime(2022,1,10), JobTitle="Financial Analyst",       DepartmentId=fin, BaseSalary=68000,  Status=EmploymentStatus.Active, City="Delhi" },
            new() { EmployeeCode="EMP015", FirstName="Nisha",    LastName="Patel",      Email="nisha.patel@ems.com",      Phone="9876543224", Gender=Gender.Female, DateOfBirth=new DateTime(1997,1,4),   JoinDate=new DateTime(2023,2,1),  JobTitle="Accounts Executive",      DepartmentId=fin, BaseSalary=48000,  Status=EmploymentStatus.Active, City="Ahmedabad" },
            new() { EmployeeCode="EMP016", FirstName="Rohan",    LastName="Joshi",      Email="rohan.joshi@ems.com",      Phone="9876543225", Gender=Gender.Male,   DateOfBirth=new DateTime(1993,4,22),  JoinDate=new DateTime(2021,8,1),  JobTitle="Digital Marketing Lead",  DepartmentId=mkt, BaseSalary=78000,  Status=EmploymentStatus.Active, City="Mumbai" },
            new() { EmployeeCode="EMP017", FirstName="Tanya",    LastName="Malhotra",   Email="tanya.malhotra@ems.com",   Phone="9876543226", Gender=Gender.Female, DateOfBirth=new DateTime(1998,9,14),  JoinDate=new DateTime(2023,4,15), JobTitle="Content Writer",          DepartmentId=mkt, BaseSalary=42000,  Status=EmploymentStatus.Active, City="Gurgaon" },
            new() { EmployeeCode="EMP018", FirstName="Sanjay",   LastName="Reddy",      Email="sanjay.reddy@ems.com",     Phone="9876543227", Gender=Gender.Male,   DateOfBirth=new DateTime(1985,6,7),   JoinDate=new DateTime(2018,3,1),  JobTitle="Operations Manager",      DepartmentId=ops, BaseSalary=105000, Status=EmploymentStatus.Active, City="Hyderabad" },
            new() { EmployeeCode="EMP019", FirstName="Kavya",    LastName="Menon",      Email="kavya.menon@ems.com",      Phone="9876543228", Gender=Gender.Female, DateOfBirth=new DateTime(1996,11,30), JoinDate=new DateTime(2022,7,1),  JobTitle="Process Analyst",         DepartmentId=ops, BaseSalary=62000,  Status=EmploymentStatus.Active, City="Kochi" },
            new() { EmployeeCode="EMP020", FirstName="Prakash",  LastName="Kumar",      Email="prakash.kumar@ems.com",    Phone="9876543229", Gender=Gender.Male,   DateOfBirth=new DateTime(1991,2,18),  JoinDate=new DateTime(2020,10,1), JobTitle="Logistics Coordinator",   DepartmentId=ops, BaseSalary=55000,  Status=EmploymentStatus.Active, City="Chennai" },
            new() { EmployeeCode="EMP021", FirstName="Divya",    LastName="Saxena",     Email="divya.saxena@ems.com",     Phone="9876543230", Gender=Gender.Female, DateOfBirth=new DateTime(1992,7,25),  JoinDate=new DateTime(2021,3,15), JobTitle="Sales Manager",           DepartmentId=sal, BaseSalary=90000,  Status=EmploymentStatus.Active, City="Delhi" },
            new() { EmployeeCode="EMP022", FirstName="Arun",     LastName="Nambiar",    Email="arun.nambiar@ems.com",     Phone="9876543231", Gender=Gender.Male,   DateOfBirth=new DateTime(1994,5,9),   JoinDate=new DateTime(2022,6,1),  JobTitle="Sales Executive",         DepartmentId=sal, BaseSalary=58000,  Status=EmploymentStatus.Active, City="Bangalore" },
            new() { EmployeeCode="EMP023", FirstName="Pooja",    LastName="Agarwal",    Email="pooja.agarwal@ems.com",    Phone="9876543232", Gender=Gender.Female, DateOfBirth=new DateTime(1997,3,16),  JoinDate=new DateTime(2023,1,10), JobTitle="Sales Executive",         DepartmentId=sal, BaseSalary=52000,  Status=EmploymentStatus.Active, City="Lucknow" },
            new() { EmployeeCode="EMP024", FirstName="Manish",   LastName="Tiwari",     Email="manish.tiwari@ems.com",    Phone="9876543233", Gender=Gender.Male,   DateOfBirth=new DateTime(1990,8,21),  JoinDate=new DateTime(2020,5,1),  JobTitle="Key Account Manager",     DepartmentId=sal, BaseSalary=82000,  Status=EmploymentStatus.Active, City="Pune" },
            new() { EmployeeCode="EMP025", FirstName="Shreya",   LastName="Bhatt",      Email="shreya.bhatt@ems.com",     Phone="9876543234", Gender=Gender.Female, DateOfBirth=new DateTime(1999,12,5),  JoinDate=new DateTime(2023,8,1),  JobTitle="Sales Trainee",           DepartmentId=sal, BaseSalary=35000,  Status=EmploymentStatus.Active, City="Surat" },
        };
        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        // Reporting hierarchy
        var e = employees;
        e[6].ReportsToId  = e[0].Id; e[7].ReportsToId  = e[0].Id; e[8].ReportsToId  = e[0].Id; e[9].ReportsToId  = e[7].Id;
        e[10].ReportsToId = e[1].Id; e[11].ReportsToId = e[1].Id;
        e[12].ReportsToId = e[2].Id; e[13].ReportsToId = e[2].Id; e[14].ReportsToId = e[12].Id;
        e[15].ReportsToId = e[3].Id; e[16].ReportsToId = e[3].Id;
        e[17].ReportsToId = e[4].Id; e[18].ReportsToId = e[4].Id; e[19].ReportsToId = e[17].Id;
        e[20].ReportsToId = e[5].Id; e[21].ReportsToId = e[5].Id; e[22].ReportsToId = e[5].Id; e[23].ReportsToId = e[5].Id; e[24].ReportsToId = e[5].Id;
        await context.SaveChangesAsync();

        // Department managers
        departments[0].ManagerId = e[0].Id; departments[1].ManagerId = e[1].Id; departments[2].ManagerId = e[2].Id;
        departments[3].ManagerId = e[3].Id; departments[4].ManagerId = e[4].Id; departments[5].ManagerId = e[5].Id;
        await context.SaveChangesAsync();

        // Leave requests
        context.LeaveRequests.AddRange(
            new LeaveRequest { EmployeeId=e[6].Id,  LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(5),  EndDate=DateTime.Today.AddDays(9),  Reason="Family vacation",   Status=LeaveStatus.Pending },
            new LeaveRequest { EmployeeId=e[7].Id,  LeaveType=LeaveType.Sick,    StartDate=DateTime.Today.AddDays(-3), EndDate=DateTime.Today.AddDays(-1), Reason="Fever and cold",    Status=LeaveStatus.ApprovedByHR },
            new LeaveRequest { EmployeeId=e[10].Id, LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(10), EndDate=DateTime.Today.AddDays(14), Reason="Personal travel",   Status=LeaveStatus.ApprovedByManager },
            new LeaveRequest { EmployeeId=e[13].Id, LeaveType=LeaveType.Sick,    StartDate=DateTime.Today.AddDays(-1), EndDate=DateTime.Today,             Reason="Medical checkup",   Status=LeaveStatus.Pending },
            new LeaveRequest { EmployeeId=e[20].Id, LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(20), EndDate=DateTime.Today.AddDays(25), Reason="Wedding ceremony",  Status=LeaveStatus.Pending },
            new LeaveRequest { EmployeeId=e[8].Id,  LeaveType=LeaveType.Unpaid,  StartDate=DateTime.Today.AddDays(30), EndDate=DateTime.Today.AddDays(35), Reason="Personal reasons",  Status=LeaveStatus.RejectedByManager },
            new LeaveRequest { EmployeeId=e[16].Id, LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(7),  EndDate=DateTime.Today.AddDays(11), Reason="Festival holidays", Status=LeaveStatus.Pending }
        );
        await context.SaveChangesAsync();

        // Users
        await CreateUserAsync(userManager, "admin@ems.com",           "Admin@123!",  "System Administrator", null,    "Admin");
        await CreateUserAsync(userManager, "anjali.verma@ems.com",    "Hr@123!",     "Anjali Verma",         e[1].Id, "HR");
        await CreateUserAsync(userManager, "arjun.mehta@ems.com",     "Mgr@123!",    "Arjun Mehta",          e[0].Id, "Manager");
        await CreateUserAsync(userManager, "sneha.kapoor@ems.com",    "Emp@123!",    "Sneha Kapoor",         e[6].Id, "Employee");
        await CreateUserAsync(userManager, "ravi.choudhary@ems.com",  "Mgr@123!",    "Ravi Choudhary",       e[4].Id, "Manager");
        await CreateUserAsync(userManager, "nikhil.desai@ems.com",    "Mgr@123!",    "Nikhil Desai",         e[5].Id, "Manager");
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> um, string email, string password, string fullName, int? employeeId, string role)
    {
        if (await um.FindByEmailAsync(email) != null) return;
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmployeeId = employeeId, EmailConfirmed = true };
        var result = await um.CreateAsync(user, password);
        if (result.Succeeded) await um.AddToRoleAsync(user, role);
    }
}
