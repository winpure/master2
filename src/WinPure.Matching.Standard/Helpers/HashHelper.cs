using System.Security.Cryptography;
using System.Text;

namespace WinPure.Matching.Helpers;

public static class HashHelper
{
    internal static string GetHash(string data)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            var byteData = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(byteData);
        }
    }
}