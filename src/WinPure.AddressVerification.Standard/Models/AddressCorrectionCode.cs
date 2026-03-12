namespace WinPure.AddressVerification.Models;

[Serializable]
public class AddressCorrectionCode
{
    /// <summary>
    /// Correction code (from external API)
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Code description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// The number of occurrences of this code in the data
    /// </summary>
    public int Count { get; set; }
}