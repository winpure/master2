using WinPure.AddressVerification.Models;

namespace WinPure.AddressVerification.Services;

internal interface IUsAddressVerificationService
{
    Task<AddressVerificationReport> VerifyAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1);
}