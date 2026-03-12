using System.Reflection;

namespace WinPure.AddressVerification.Models;

/// <summary>
/// Statistic information about address verification
/// </summary>
[Serializable]
[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class AddressVerificationReport
{
    public AddressVerificationReport()
    {
        Correction = new List<AddressCorrectionCode>();
        CommonData = new AddressVerificationStatistic();
    }

    /// <summary>
    /// Total verification time
    /// </summary>
    public TimeSpan VerifyTime { get; set; }

    /// <summary>
    /// Full list of correction codes for addresses with description
    /// </summary>
    public List<AddressCorrectionCode> Correction { get; set; }

    /// <summary>
    /// Total statistic of verification
    /// </summary>
    public AddressVerificationStatistic CommonData { get; set; }

    /// <summary>
    /// How much credits was used (for online verification only)
    /// </summary>
    public int UsedCredits { get; set; }
}