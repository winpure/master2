namespace WinPure.Common.Services;

internal interface IEncryptionService
{
    string Hash(string value);
    string Decrypt(string cipherText);
    string Encrypt(string text);
}