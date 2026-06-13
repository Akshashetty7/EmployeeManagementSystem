namespace EmsApi.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FullName,
    string Role
);

public record RefreshRequest(string RefreshToken);
