using System.Collections.Generic;

namespace WinPure.AddressVerification.Models;

/// <summary>
/// Base address verification class
/// </summary>
[Serializable]
public abstract class AddressVerificationSettings
{
    /// <summary>
    /// 
    /// </summary>
    protected AddressVerificationSettings()
    {
        AddressColumns = new List<string>();
        LocalityColumns = new List<string>();
        PostalCodeColumns = new List<string>();
        StateColumns = new List<string>();
        SelectedRows = new List<long>();
    }

    /// <summary>
    /// Contains number of rows from WinPurePrimK field if only partly verification required. If empty - all records would be verified.
    /// </summary>
    public List<long> SelectedRows { get; set; }
    /// <summary>
    /// list of columns which contains address data
    /// </summary>
    public List<string> AddressColumns { get; set; }
    /// <summary>
    /// list of columns which contains postal code
    /// </summary>
    public List<string> PostalCodeColumns { get; set; }
    /// <summary>
    /// list of columns which contains locality
    /// </summary>
    public List<string> LocalityColumns { get; set; }
    /// <summary>
    /// list of columns which contains state
    /// </summary>
    public List<string> StateColumns { get; set; }
    /// <summary>
    /// Parse geo tag.
    /// </summary>
    public bool GeoTag { get; set; }        
    /// <summary>
    /// Path to local database with address data (required only for local verification)
    /// </summary>
    public string PathToDb { get; set; }
}