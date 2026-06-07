using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "HR", "Manager", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (context.Departments.Any()) return; // Already seeded

            // Seed Departments
            var departments = new List<Department>
            {
                new() { Id = 1,  Name = "Engineering",        Description = "Software development and architecture", Location = "Floor 3", IsActive = true },
                new() { Id = 2,  Name = "Human Resources",    Description = "Talent acquisition and management",    Location = "Floor 1", IsActive = true },
                new() { Id = 3,  Name = "Finance",            Description = "Financial planning and accounting",    Location = "Floor 2", IsActive = true },
                new() { Id = 4,  Name = "Marketing",          Description = "Brand and product marketing",          Location = "Floor 2", IsActive = true },
                new() { Id = 5,  Name = "Operations",         Description = "Business operations and logistics",    Location = "Floor 1", IsActive = true },
                new() { Id = 6,  Name = "Sales",              Description = "Revenue generation and client relations", Location = "Floor 3", IsActive = true },
            };
            context.Departments.AddRange(departments);
            await context.SaveChangesAsync();

            // Seed 25 Employees
            var employees = new List<Employee>
            {
                // Engineering (Dept 1)
                new() { Id=1,  EmployeeCode="EMP001", FirstName="Arjun",    LastName="Mehta",     Email="arjun.mehta@nexacorp.com",   Phone="9876543210", Gender=Gender.Male,   DateOfBirth=new DateTime(1985,3,15),  JoinDate=new DateTime(2018,1,10), JobTitle="CTO",                   DepartmentId=1, BaseSalary=250000, Status=EmploymentStatus.Active, City="Bangalore", Address="12 MG Road", PostalCode="560001", NationalId="ABCDE1234F", EmergencyContact="Priya Mehta: 9876500000" },
                new() { Id=2,  EmployeeCode="EMP002", FirstName="Sneha",    LastName="Kapoor",    Email="sneha.kapoor@nexacorp.com",   Phone="9876543211", Gender=Gender.Female, DateOfBirth=new DateTime(1990,7,22),  JoinDate=new DateTime(2019,4,15), JobTitle="Senior Developer",       DepartmentId=1, BaseSalary=180000, Status=EmploymentStatus.Active, City="Bangalore", Address="45 Indiranagar", PostalCode="560038", NationalId="FGHIJ5678K" },
                new() { Id=3,  EmployeeCode="EMP003", FirstName="Rahul",    LastName="Sharma",    Email="rahul.sharma@nexacorp.com",   Phone="9876543212", Gender=Gender.Male,   DateOfBirth=new DateTime(1992,11,5),  JoinDate=new DateTime(2020,6,1),  JobTitle="Full Stack Developer",   DepartmentId=1, BaseSalary=140000, Status=EmploymentStatus.Active, City="Bangalore", Address="7 Koramangala",  PostalCode="560034", NationalId="KLMNO9012P" },
                new() { Id=4,  EmployeeCode="EMP004", FirstName="Priya",    LastName="Nair",      Email="priya.nair@nexacorp.com",     Phone="9876543213", Gender=Gender.Female, DateOfBirth=new DateTime(1994,2,18),  JoinDate=new DateTime(2021,2,20), JobTitle="QA Engineer",            DepartmentId=1, BaseSalary=120000, Status=EmploymentStatus.Active, City="Bangalore", Address="89 HSR Layout",  PostalCode="560102", NationalId="PQRST3456U" },
                new() { Id=5,  EmployeeCode="EMP005", FirstName="Vikram",   LastName="Singh",     Email="vikram.singh@nexacorp.com",   Phone="9876543214", Gender=Gender.Male,   DateOfBirth=new DateTime(1988,9,30),  JoinDate=new DateTime(2019,9,10), JobTitle="DevOps Engineer",        DepartmentId=1, BaseSalary=160000, Status=EmploymentStatus.Active, City="Bangalore", Address="33 Whitefield",  PostalCode="560066", NationalId="UVWXY7890Z" },

                // HR (Dept 2)
                new() { Id=6,  EmployeeCode="EMP006", FirstName="Anjali",   LastName="Verma",     Email="anjali.verma@nexacorp.com",   Phone="9876543215", Gender=Gender.Female, DateOfBirth=new DateTime(1987,5,12),  JoinDate=new DateTime(2017,3,1),  JobTitle="HR Manager",             DepartmentId=2, BaseSalary=160000, Status=EmploymentStatus.Active, City="Bangalore", Address="21 Jayanagar",   PostalCode="560041", NationalId="ABCDF2345G" },
                new() { Id=7,  EmployeeCode="EMP007", FirstName="Rohan",    LastName="Joshi",     Email="rohan.joshi@nexacorp.com",    Phone="9876543216", Gender=Gender.Male,   DateOfBirth=new DateTime(1993,8,27),  JoinDate=new DateTime(2021,7,5),  JobTitle="HR Executive",           DepartmentId=2, BaseSalary=85000,  Status=EmploymentStatus.Active, City="Bangalore", Address="5 BTM Layout",   PostalCode="560076", NationalId="HIJKL6789M" },
                new() { Id=8,  EmployeeCode="EMP008", FirstName="Kavya",    LastName="Reddy",     Email="kavya.reddy@nexacorp.com",    Phone="9876543217", Gender=Gender.Female, DateOfBirth=new DateTime(1996,1,14),  JoinDate=new DateTime(2022,1,17), JobTitle="HR Coordinator",         DepartmentId=2, BaseSalary=70000,  Status=EmploymentStatus.Active, City="Bangalore", Address="11 Electronic City", PostalCode="560100", NationalId="MNOPQ0123R" },

                // Finance (Dept 3)
                new() { Id=9,  EmployeeCode="EMP009", FirstName="Suresh",   LastName="Pillai",    Email="suresh.pillai@nexacorp.com",  Phone="9876543218", Gender=Gender.Male,   DateOfBirth=new DateTime(1980,6,19),  JoinDate=new DateTime(2016,5,10), JobTitle="CFO",                    DepartmentId=3, BaseSalary=230000, Status=EmploymentStatus.Active, City="Bangalore", Address="66 Malleswaram", PostalCode="560003", NationalId="RSTUV4567W" },
                new() { Id=10, EmployeeCode="EMP010", FirstName="Deepika",  LastName="Iyer",      Email="deepika.iyer@nexacorp.com",   Phone="9876543219", Gender=Gender.Female, DateOfBirth=new DateTime(1991,4,8),   JoinDate=new DateTime(2019,11,12),JobTitle="Finance Analyst",        DepartmentId=3, BaseSalary=115000, Status=EmploymentStatus.Active, City="Bangalore", Address="30 Rajajinagar",  PostalCode="560010", NationalId="WXYZ12345A" },
                new() { Id=11, EmployeeCode="EMP011", FirstName="Mohan",    LastName="Krishnan",  Email="mohan.krishnan@nexacorp.com", Phone="9876543220", Gender=Gender.Male,   DateOfBirth=new DateTime(1989,12,25), JoinDate=new DateTime(2020,3,15), JobTitle="Accounts Manager",       DepartmentId=3, BaseSalary=130000, Status=EmploymentStatus.Active, City="Bangalore", Address="14 Basavanagudi", PostalCode="560004", NationalId="BCDEF6789G" },
                new() { Id=12, EmployeeCode="EMP012", FirstName="Nisha",    LastName="Patel",     Email="nisha.patel@nexacorp.com",    Phone="9876543221", Gender=Gender.Female, DateOfBirth=new DateTime(1995,7,3),   JoinDate=new DateTime(2022,6,20), JobTitle="Junior Accountant",      DepartmentId=3, BaseSalary=65000,  Status=EmploymentStatus.Active, City="Bangalore", Address="9 Banashankari",  PostalCode="560050", NationalId="HIJKL0123M" },

                // Marketing (Dept 4)
                new() { Id=13, EmployeeCode="EMP013", FirstName="Aditya",   LastName="Bose",      Email="aditya.bose@nexacorp.com",    Phone="9876543222", Gender=Gender.Male,   DateOfBirth=new DateTime(1986,10,11), JoinDate=new DateTime(2018,8,1),  JobTitle="Marketing Director",     DepartmentId=4, BaseSalary=195000, Status=EmploymentStatus.Active, City="Bangalore", Address="77 Domlur",       PostalCode="560071", NationalId="NOPQR4567S" },
                new() { Id=14, EmployeeCode="EMP014", FirstName="Meera",    LastName="Gupta",     Email="meera.gupta@nexacorp.com",    Phone="9876543223", Gender=Gender.Female, DateOfBirth=new DateTime(1993,3,29),  JoinDate=new DateTime(2020,10,5), JobTitle="Content Strategist",     DepartmentId=4, BaseSalary=100000, Status=EmploymentStatus.Active, City="Bangalore", Address="2 Ulsoor",        PostalCode="560042", NationalId="TUVWX8901Y" },
                new() { Id=15, EmployeeCode="EMP015", FirstName="Kiran",    LastName="Rao",       Email="kiran.rao@nexacorp.com",      Phone="9876543224", Gender=Gender.Male,   DateOfBirth=new DateTime(1997,9,6),   JoinDate=new DateTime(2023,1,9),  JobTitle="Digital Marketing Exec", DepartmentId=4, BaseSalary=72000,  Status=EmploymentStatus.Active, City="Bangalore", Address="55 Frazer Town",  PostalCode="560005", NationalId="ZABCD2345E" },

                // Operations (Dept 5)
                new() { Id=16, EmployeeCode="EMP016", FirstName="Ravi",     LastName="Choudhary", Email="ravi.choudhary@nexacorp.com", Phone="9876543225", Gender=Gender.Male,   DateOfBirth=new DateTime(1983,1,17),  JoinDate=new DateTime(2015,6,15), JobTitle="Operations Head",        DepartmentId=5, BaseSalary=210000, Status=EmploymentStatus.Active, City="Bangalore", Address="40 Hebbal",       PostalCode="560024", NationalId="FGHIJ6789K" },
                new() { Id=17, EmployeeCode="EMP017", FirstName="Pooja",    LastName="Mishra",    Email="pooja.mishra@nexacorp.com",   Phone="9876543226", Gender=Gender.Female, DateOfBirth=new DateTime(1990,5,23),  JoinDate=new DateTime(2019,2,1),  JobTitle="Process Analyst",        DepartmentId=5, BaseSalary=105000, Status=EmploymentStatus.Active, City="Bangalore", Address="18 Yeshwanthpur", PostalCode="560022", NationalId="KLMNO0123P" },
                new() { Id=18, EmployeeCode="EMP018", FirstName="Sanjay",   LastName="Kumar",     Email="sanjay.kumar@nexacorp.com",   Phone="9876543227", Gender=Gender.Male,   DateOfBirth=new DateTime(1988,8,10),  JoinDate=new DateTime(2020,7,20), JobTitle="Supply Chain Manager",   DepartmentId=5, BaseSalary=125000, Status=EmploymentStatus.OnLeave, City="Bangalore", Address="3 Peenya",        PostalCode="560058", NationalId="PQRST4567U" },
                new() { Id=19, EmployeeCode="EMP019", FirstName="Divya",    LastName="Menon",     Email="divya.menon@nexacorp.com",    Phone="9876543228", Gender=Gender.Female, DateOfBirth=new DateTime(1994,11,28), JoinDate=new DateTime(2021,4,12), JobTitle="Operations Executive",   DepartmentId=5, BaseSalary=78000,  Status=EmploymentStatus.Active, City="Bangalore", Address="62 Vijayanagar",  PostalCode="560040", NationalId="UVWXY8901Z" },

                // Sales (Dept 6)
                new() { Id=20, EmployeeCode="EMP020", FirstName="Nikhil",   LastName="Desai",     Email="nikhil.desai@nexacorp.com",   Phone="9876543229", Gender=Gender.Male,   DateOfBirth=new DateTime(1984,2,7),   JoinDate=new DateTime(2016,9,1),  JobTitle="VP of Sales",            DepartmentId=6, BaseSalary=220000, Status=EmploymentStatus.Active, City="Bangalore", Address="25 Cunningham Rd",PostalCode="560052", NationalId="ABCDE2345F" },
                new() { Id=21, EmployeeCode="EMP021", FirstName="Shweta",   LastName="Agarwal",   Email="shweta.agarwal@nexacorp.com", Phone="9876543230", Gender=Gender.Female, DateOfBirth=new DateTime(1991,6,15),  JoinDate=new DateTime(2019,5,20), JobTitle="Sales Manager",          DepartmentId=6, BaseSalary=145000, Status=EmploymentStatus.Active, City="Bangalore", Address="8 Palace Road",   PostalCode="560052", NationalId="GHIJK6789L" },
                new() { Id=22, EmployeeCode="EMP022", FirstName="Tarun",    LastName="Saxena",    Email="tarun.saxena@nexacorp.com",   Phone="9876543231", Gender=Gender.Male,   DateOfBirth=new DateTime(1993,4,20),  JoinDate=new DateTime(2020,12,7), JobTitle="Sales Executive",        DepartmentId=6, BaseSalary=90000,  Status=EmploymentStatus.Active, City="Bangalore", Address="15 Residency Rd", PostalCode="560025", NationalId="MNOPQ0123R" },
                new() { Id=23, EmployeeCode="EMP023", FirstName="Lakshmi",  LastName="Subramaniam",Email="lakshmi.s@nexacorp.com",     Phone="9876543232", Gender=Gender.Female, DateOfBirth=new DateTime(1996,3,11),  JoinDate=new DateTime(2022,3,14), JobTitle="Business Dev Executive", DepartmentId=6, BaseSalary=82000,  Status=EmploymentStatus.Active, City="Bangalore", Address="44 Richmond Rd",  PostalCode="560025", NationalId="STUVW4567X" },
                new() { Id=24, EmployeeCode="EMP024", FirstName="Amit",     LastName="Tiwari",    Email="amit.tiwari@nexacorp.com",    Phone="9876543233", Gender=Gender.Male,   DateOfBirth=new DateTime(1987,7,19),  JoinDate=new DateTime(2018,4,3),  JobTitle="Key Accounts Manager",   DepartmentId=6, BaseSalary=155000, Status=EmploymentStatus.Active, City="Bangalore", Address="6 Infantry Road", PostalCode="560001", NationalId="YZABC8901D" },
                new() { Id=25, EmployeeCode="EMP025", FirstName="Ritu",     LastName="Malhotra",  Email="ritu.malhotra@nexacorp.com",  Phone="9876543234", Gender=Gender.Female, DateOfBirth=new DateTime(1998,10,2),  JoinDate=new DateTime(2023,7,3),  JobTitle="Sales Trainee",          DepartmentId=6, BaseSalary=55000,  Status=EmploymentStatus.Active, City="Bangalore", Address="19 Lavelle Rd",   PostalCode="560001", NationalId="EFGHI2345J" },
            };

            // Set reporting structure
            employees[1].ReportsToId = 1;  // Sneha → Arjun (CTO)
            employees[2].ReportsToId = 1;
            employees[3].ReportsToId = 2;
            employees[4].ReportsToId = 2;
            employees[6].ReportsToId = 6;  // Rohan → Anjali
            employees[7].ReportsToId = 6;
            employees[9].ReportsToId = 9;  // Deepika → Suresh
            employees[10].ReportsToId = 9;
            employees[11].ReportsToId = 9;
            employees[13].ReportsToId = 13; // Meera → Aditya
            employees[14].ReportsToId = 13;
            employees[16].ReportsToId = 16; // Pooja → Ravi
            employees[17].ReportsToId = 16;
            employees[18].ReportsToId = 16;
            employees[20].ReportsToId = 20; // Shweta → Nikhil
            employees[21].ReportsToId = 21;
            employees[22].ReportsToId = 21;
            employees[23].ReportsToId = 21;
            employees[24].ReportsToId = 20;

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            // Assign dept managers
            departments[0].ManagerId = 1;  // Arjun = Engineering head
            departments[1].ManagerId = 6;  // Anjali = HR head
            departments[2].ManagerId = 9;  // Suresh = Finance head
            departments[3].ManagerId = 13; // Aditya = Marketing head
            departments[4].ManagerId = 16; // Ravi = Ops head
            departments[5].ManagerId = 20; // Nikhil = Sales head
            await context.SaveChangesAsync();

            // Seed Sample Leave Requests
            var leaves = new List<LeaveRequest>
            {
                new() { EmployeeId=18, LeaveType=LeaveType.Sick,    StartDate=DateTime.Today.AddDays(-5), EndDate=DateTime.Today.AddDays(2),  Reason="Medical procedure recovery",         Status=LeaveStatus.ApprovedByHR,        ManagerRemarks="Approved",           HRRemarks="Approved with pay" },
                new() { EmployeeId=4,  LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(5),  EndDate=DateTime.Today.AddDays(9),  Reason="Family vacation",                    Status=LeaveStatus.ApprovedByManager,   ManagerRemarks="Approved, enjoy!" },
                new() { EmployeeId=15, LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(10), EndDate=DateTime.Today.AddDays(13), Reason="Personal work",                      Status=LeaveStatus.Pending },
                new() { EmployeeId=8,  LeaveType=LeaveType.Sick,    StartDate=DateTime.Today.AddDays(-2), EndDate=DateTime.Today,             Reason="Fever and cold",                     Status=LeaveStatus.Pending },
                new() { EmployeeId=23, LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(20), EndDate=DateTime.Today.AddDays(24), Reason="Wedding function",                   Status=LeaveStatus.Pending },
                new() { EmployeeId=12, LeaveType=LeaveType.Maternity,StartDate=DateTime.Today.AddDays(30),EndDate=DateTime.Today.AddDays(120),Reason="Maternity leave",                    Status=LeaveStatus.Pending },
                new() { EmployeeId=3,  LeaveType=LeaveType.Annual,  StartDate=DateTime.Today.AddDays(-30),EndDate=DateTime.Today.AddDays(-25),Reason="Annual leave",                       Status=LeaveStatus.ApprovedByHR },
            };
            context.LeaveRequests.AddRange(leaves);
            await context.SaveChangesAsync();

            // Create system users
            await CreateUserAsync(userManager, "admin@nexacorp.com",    "Admin@123!",   "System Admin",   "Admin",    null);
            await CreateUserAsync(userManager, "anjali.verma@nexacorp.com", "Hr@123!", "Anjali Verma",    "HR",       6);
            await CreateUserAsync(userManager, "arjun.mehta@nexacorp.com",  "Mgr@123!","Arjun Mehta",    "Manager",  1);
            await CreateUserAsync(userManager, "sneha.kapoor@nexacorp.com", "Emp@123!", "Sneha Kapoor",   "Employee", 2);
            await CreateUserAsync(userManager, "ravi.choudhary@nexacorp.com","Mgr@123!","Ravi Choudhary","Manager",  16);
            await CreateUserAsync(userManager, "nikhil.desai@nexacorp.com",  "Mgr@123!","Nikhil Desai",  "Manager",  20);
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager,
            string email, string password, string fullName, string role, int? employeeId)
        {
            if (await userManager.FindByEmailAsync(email) != null) return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmployeeId = employeeId,
                EmailConfirmed = true,
                IsActive = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
