using System.Security.Cryptography;
using System.Text;

namespace IoTBay.Utils;

public class HashUtils
{
    private const int SaltSize = 128;
    private const int HashSize = 256;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public static string HashPassword(string plainTextPassword, out string salt)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize / 2);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(plainTextPassword), 
            saltBytes, 
            Iterations, 
            Algorithm, 
            HashSize / 2);
        
        salt = Convert.ToHexString(saltBytes);
        
        Console.WriteLine(Convert.ToHexString(hashBytes).Length);
        return Convert.ToHexString(hashBytes);
    }

    public static bool VerifyPassword(string hashedPassword, string plainTextPassword, string salt)
    {
        var saltBytes = Convert.FromHexString(salt);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(plainTextPassword),
            saltBytes,
            Iterations,
            Algorithm,
            HashSize / 2);
        return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(hashedPassword), hashBytes);
    }
}