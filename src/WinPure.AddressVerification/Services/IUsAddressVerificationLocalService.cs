namespace WinPure.AddressVerification.Services;

internal interface IUsAddressVerificationLocalService
{
    AddressVerificationReport VerifyAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1);
}