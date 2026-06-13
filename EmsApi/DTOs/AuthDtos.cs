namespace EmsApi.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FullName,
    string Role,
    IList<string> Roles
);

public record RefreshRequest(string RefreshToken);

public class EmployeeImportRow
{
    public int RowNumber { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Name { get; set; }
    public string Status { get; set; } = "Success";
    public string? Error { get; set; }
}
