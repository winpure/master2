using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WinPure.Common.DependencyInjection;

internal partial class WinPureCommonDependency
{
    private class EncryptionService : IEncryptionService
    {
        private static string _key = "NfHG7-82HXYUnpa)8%0dJevlm, xJulm";
        //private static string _key = "3R[SZK))^0kals;[W/&sOC%nvnr#jr,P@j#w;;9"; new randon extra key

        public string Hash(string value)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(_key),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                var key = Encoding.UTF8.GetBytes(_key);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(value);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            var str = Convert.ToBase64String(result);
                            var fullCipher = Convert.FromBase64String(str);
                            return str;
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                value = value.Replace(" ", "+");
                var fullCipher = Convert.FromBase64String(value);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
                var key = Encoding.UTF8.GetBytes(_key);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}