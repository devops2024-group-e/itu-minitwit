using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace itu_new_minitwit.Utils;

public static class PasswordHash
{
    public static string Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(16);

        return KeyDerivation.Pbkdf2(
            password: value,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 600000,
            numBytesRequested: 32)?.ToString() ?? string.Empty;
    }
}
