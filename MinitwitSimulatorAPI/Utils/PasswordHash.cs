using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MinitwitSimulatorAPI.Utils;

public static class PasswordHash
{
    public static string Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return GeneratePasswordHash(value, salt);
    }

    private static string GeneratePasswordHash(string password, byte[] salt)
    {
        var hashedResult = Convert.ToHexString(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 100000,
                    numBytesRequested: 32));
        return $"{Convert.ToHexString(salt)}${hashedResult}";
    }

    public static bool CheckPasswordHash(string password, string hashedPassword)
    {
        var passwordParts = hashedPassword.Split('$');

        if (passwordParts.Length != 2)
            return false;
        var pp = GeneratePasswordHash(password, Convert.FromHexString(passwordParts[0]));
        return pp == hashedPassword;
    }
}
