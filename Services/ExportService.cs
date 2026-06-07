using ClosedXML.Excel;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IExportService
    {
        byte[] ExportEmployeesToExcel(IEnumerable<Employee> employees);
        byte[] ExportLeaveReportToExcel(IEnumerable<LeaveRequest> leaves);
    }

    public class ExportService : IExportService
    {
        public byte[] ExportEmployeesToExcel(IEnumerable<Employee> employees)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Employees");

            // Header
            string[] headers = { "Code", "Name", "Email", "Phone", "Department", "Job Title", "Join Date", "Salary (INR)", "Status", "City" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e3a5f");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 2;
            foreach (var e in employees)
            {
                ws.Cell(row, 1).Value = e.EmployeeCode;
                ws.Cell(row, 2).Value = e.FullName;
                ws.Cell(row, 3).Value = e.Email;
                ws.Cell(row, 4).Value = e.Phone;
                ws.Cell(row, 5).Value = e.Department?.Name ?? "-";
                ws.Cell(row, 6).Value = e.JobTitle;
                ws.Cell(row, 7).Value = e.JoinDate.ToString("dd MMM yyyy");
                ws.Cell(row, 8).Value = e.BaseSalary;
                ws.Cell(row, 9).Value = e.Status.ToString();
                ws.Cell(row, 10).Value = e.City;

                if (row % 2 == 0)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f0f4f8");

                row++;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportLeaveReportToExcel(IEnumerable<LeaveRequest> leaves)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Leave Report");

            string[] headers = { "Employee", "Department", "Leave Type", "Start Date", "End Date", "Days", "Status", "Applied On" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e3a5f");
                cell.Style.Font.FontColor = XLColor.White;
            }

            int row = 2;
            foreach (var l in leaves)
            {
                ws.Cell(row, 1).Value = l.Employee?.FullName ?? "-";
                ws.Cell(row, 2).Value = l.Employee?.Department?.Name ?? "-";
                ws.Cell(row, 3).Value = l.LeaveType.ToString();
                ws.Cell(row, 4).Value = l.StartDate.ToString("dd MMM yyyy");
                ws.Cell(row, 5).Value = l.EndDate.ToString("dd MMM yyyy");
                ws.Cell(row, 6).Value = (l.EndDate - l.StartDate).Days + 1;
                ws.Cell(row, 7).Value = l.Status.ToString();
                ws.Cell(row, 8).Value = l.CreatedAt.ToString("dd MMM yyyy");
                row++;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
