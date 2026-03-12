using WinPure.AddressVerification.Models;
using WinPure.Common.Abstractions;
using WinPure.Common.Models;

namespace WinPure.AddressVerification.Services;

internal interface IAddressVerificationService : IWinPureNotification
{
    AddressVerificationReport VerifyAddresses(AddressVerificationSettings verificationSettings, ImportedDataInfo importedDataInfo, DataTable tblOptions, int recordsForVerification, CancellationToken cancellationToken);
}