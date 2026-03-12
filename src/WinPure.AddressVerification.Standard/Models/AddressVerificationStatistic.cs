namespace WinPure.AddressVerification.Models;

[Serializable]
public class AddressVerificationStatistic
{
    /// <summary>
    /// Name of the data table for verification
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// Number of the records in the data
    /// </summary>
    public int TotalRecords { get; set; }
    /// <summary>
    /// Number of success verified records
    /// </summary>
    public int AddressSuccess { get; set; }
    /// <summary>
    /// Percentage of success verified records
    /// </summary>
    public double AddressSuccessPercent { get; set; }
    /// <summary>
    /// Count of success verified Geo codes
    /// </summary>
    public int GeoCodeSuccess { get; set; }
    /// <summary>
    /// Percentage of success verified geo codes
    /// </summary>
    public double GeoCodeSuccessPercent { get; set; }
}