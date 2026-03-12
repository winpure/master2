namespace WinPure.AddressVerification.Models;

/// <summary>
///  Address verification settings for USA and other countries
/// </summary>
[Serializable]
public class UsAddressVerificationSettings : AddressVerificationSettings
{
    /// <summary>
    /// 
    /// </summary>
    public string Country { get; set; }
    /// <summary>
    /// Should verify and parse address
    /// </summary>
    public bool Verification { get; set; }
    /// <summary>
    /// Should  parse serp data
    /// </summary>
    public bool SerpField { get; set; }
    /// <summary>
    /// Should  parse amas data
    /// </summary>
    public bool AmasField { get; set; }
    /// <summary>
    /// Should  parse cass data
    /// </summary>
    public bool CassField { get; set; }
    /// <summary>
    /// Use online verification (license key required)
    /// </summary>
    public bool IsOnlineVerification { get; set; }
    /// <summary>
    /// License key for online verification
    /// </summary>
    public string LicenseKey { get; set; }
    /// <summary>
    /// Amount of credits for online verification which can be used.
    /// </summary>
    public int AvailableCredits { get; set; }
    /// <summary>
    /// Column with Latitude for reverse geocode
    /// </summary>
    public string LatitudeColumn { get; set; }
    /// <summary>
    /// Column with Longitude for reverse geocode
    /// </summary>
    public string LongitudeColumn { get; set; }
    /// <summary>
    /// address verification using reverse geocode method. Can not be used together with any other verifications.
    /// Require LatitudeColumn and LongitudeColumn columns
    /// </summary>
    public bool ReverseGeocode { get; set; }
}