using System.Collections.Generic;

namespace WinPure.Cleansing.Models;

/// <summary>
/// Settings for splitting address and gender into parts using loqate library
/// </summary>
[Serializable]
public class AddressAndGenderSplitSettings : BaseCleansingSettings
{
    public AddressAndGenderSplitSettings()
    {
        GenderColumns = new List<string>();
        AddressColumns = new List<string>();
        CountryColumns = new List<string>();
        CityColumns = new List<string>();
        PostcodeColumns = new List<string>();
        RegionColumns = new List<string>();

    }
    /// <summary>
    /// List of the columns which contains all gender information
    /// </summary>
    public List<string> GenderColumns { get; set; }
    /// <summary>
    /// List of the columns which contains all address information (except ZIP or postal code data)
    /// </summary>
    public List<string> AddressColumns { get; set; }
    /// <summary>
    /// List of columns with country. Used together with AddressColumns
    /// </summary>
    public List<string> CountryColumns { get; set; }
    /// <summary>
    /// List of the columns which contains city name
    /// </summary>
    public List<string> CityColumns { get; set; }
    /// <summary>
    /// List of columns  which contains region or state
    /// </summary>
    public List<string> RegionColumns { get; set; }
    /// <summary>
    /// List of columns  which contains ZIP or Postcode
    /// </summary>
    public List<string> PostcodeColumns { get; set; }

}