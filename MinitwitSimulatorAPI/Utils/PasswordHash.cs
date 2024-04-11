using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MinitwitSimulatorAPI.Utils;

public static class PasswordHash
{
    public static string Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return GeneratePasswordHash(value, salt, "SHA512"); // TODO: Change back to 512
    }

    private static string GeneratePasswordHash(string password, byte[] salt, string algorithm)
    {
        string hashedResult;
        string result;
        switch (algorithm)
        {
            case "SHA512":
                hashedResult = Convert.ToHexString(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 21000,
                    numBytesRequested: 32));
                result = $"{Convert.ToHexString(salt)}${hashedResult}";
                break;
            default:
                hashedResult = Convert.ToHexString(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 600000,
                    numBytesRequested: 32));
                result = $"SHA512${Convert.ToHexString(salt)}${hashedResult}";
                break;
        }

        return result;
    }

    public static bool CheckPasswordHash(string password, string hashedPassword)
    {
        var passwordParts = hashedPassword.Split('$');

        string pp;

        switch (passwordParts[0])
        {
            case "SHA512":
                pp = GeneratePasswordHash(password, Convert.FromHexString(passwordParts[1]), "SHA512");
                break;
            default:
                pp = GeneratePasswordHash(password, Convert.FromHexString(passwordParts[0]), "SHA256");
                break;
        }
        return pp == hashedPassword;
    }
}
