namespace WinPure.AddressVerification.Services;

internal interface IUsAddressVerificationInDatabaseService
{
    AddressVerificationReport VerifyAddresses(string dbPath, string tableName, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1);
}