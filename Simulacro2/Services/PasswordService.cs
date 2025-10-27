using System.Security.Cryptography;
using System.Text;

namespace Simulacro2.Services;

public class PasswordService
{
    public string Hash(string plain)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string plain, string hash) => Hash(plain).Equals(hash, StringComparison.OrdinalIgnoreCase);
}