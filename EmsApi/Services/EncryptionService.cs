using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace EmsApi.Services;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string? Decrypt(string? cipherText);
    string Hash(string plainText);
}

// Problem 19: PII encryption using ASP.NET Core Data Protection.
// Keys are managed automatically by the framework — no manual key storage needed.
// Hash() produces a deterministic SHA-256 digest used for duplicate detection
// so we never need to decrypt all rows just to check uniqueness.
public class EncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public EncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("EmsApi.SensitivePII.v1");
    }

    public string Encrypt(string plainText) => _protector.Protect(plainText);

    public string? Decrypt(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return null;
        try { return _protector.Unprotect(cipherText); }
        catch { return "[protected]"; }
    }

    public string Hash(string plainText)
    {
        var normalized = plainText.Trim().ToUpperInvariant();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
