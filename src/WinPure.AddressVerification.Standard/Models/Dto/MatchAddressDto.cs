using Newtonsoft.Json;

namespace WinPure.AddressVerification.Models.Dto;

[JsonObject(Title = "Match")]
internal class MatchAddressDto : AddressVerificationData
{
    public string AVC { get; set; }
    public string AQI { get; set; }
    public string Organization { get; set; }
    public string SubBuilding { get; set; }
    public string Premise { get; set; }
    public string Building { get; set; }
    public string PostBox { get; set; }
    public string DependentThoroughfare { get; set; }
    public string Thoroughfare { get; set; }
    public string ThoroughfareName { get; set; }
    public string ThoroughfarePostDirection { get; set; }
    public string ThoroughfarePreDirection { get; set; }
    public string ThoroughfareTrailingType { get; set; }
    public string DoubleDependentLocality { get; set; }
    public string DependentLocality { get; set; }
    public string SubAdministrativeArea { get; set; }
    public string SuperAdministrativeArea { get; set; }
    public string Telephone { get; set; }
    public string CountryName { get; set; }
    public string GeoAccuracy { get; set; }
    public string GeoDistance { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Address5 { get; set; }
    public string Address6 { get; set; }
    public string Address7 { get; set; }
    public string Address8 { get; set; }


    public string AutoZoneIndicator { get; set; }
    public string BusinessIndicator { get; set; }
    public string CMRAIndicator { get; set; }
    public string CarrierRoute { get; set; }
    public string CentralizedIndicator { get; set; }
    public string CheckDigit { get; set; }
    public string CongressionalDistrict { get; set; }
    public string CurbIndicator { get; set; }
    public string DPVConfirmedIndicator { get; set; }
    public string DPVFootnotes { get; set; }
    public string DPVLACSIndicator { get; set; }
    public string DefaultFlag { get; set; }
    public string DeliveryAddress { get; set; }
    public string DeliveryAddress1 { get; set; }
    public string DeliveryPointBarCode { get; set; }
    public string DropCount { get; set; }
    public string DropSiteIndicator { get; set; }
    public string EducationalIndicator { get; set; }
    public string FIPSCountyCode { get; set; }
    public string FalsePositiveIndicator { get; set; }
    public string Finance { get; set; }
    public string Footnotes { get; set; }
    public string LACSLinkCode { get; set; }
    public string LACSLinkIndicator { get; set; }
    public string NDCBUIndicator { get; set; }
    public string NoStatIndicator { get; set; }
    public string OtherIndicator { get; set; }
    public string PMBNumber { get; set; }
    public string PMBType { get; set; }
    public string PostBoxNumber { get; set; }
    public string PostBoxNum { get; set; }
    public string PostBoxType { get; set; }
    public string PostalCodePrimary { get; set; }
    public string PostalCodeSecondary { get; set; }
    public string PostalCodeSecondaryRangeHigh { get; set; }
    public string PostalCodeSecondaryRangeLow { get; set; }
    public string PremiseNumber { get; set; }
    public string PrimaryPremise { get; set; }
    public string PrimaryAddressLine { get; set; }
    public string PrimaryNumRangeCode { get; set; }
    public string PrimaryNumRangeHigh { get; set; }
    public string PrimaryNumRangeLow { get; set; }
    public string RecordType { get; set; }
    public string ResidentialDelivery { get; set; }
    public string ReturnCode { get; set; }
    public string SUITELinkFootnote { get; set; }
    public string SeasonalIndicator { get; set; }
    public string SecondaryAddressLine { get; set; }
    public string SecondaryNumRangeCode { get; set; }
    public string SecondaryNumRangeHigh { get; set; }
    public string SecondaryNumRangeLow { get; set; }
    public string SubBuildingLeadingType { get; set; }
    public string SubBuildingNumber { get; set; }
    public string ThrowbackIndicator { get; set; }
    public string VacantIndicator { get; set; }
    public string eLOTCode { get; set; }
    public string eLOTNumber { get; set; }
    [JsonProperty("ISO3166-2")]
    public string IsoData2 { get; set; }
    [JsonProperty("ISO3166-3")]
    public string IsoData3 { get; set; }
    [JsonProperty("ISO3166-N")]
    public string IsoDataN { get; set; }

    public string MatchScore { get; set; }

    public string Barcode { get; set; }
    public string DPID { get; set; }
    public string ErrorCode { get; set; }
    public string FloorNumber { get; set; }
    public string FloorType { get; set; }
    public string LotNumber { get; set; }
    public string PreSortZone { get; set; }
    public string SecondaryPremise { get; set; }
    public string SecondaryPremiseSuffix { get; set; }
    public string PostBoxNumberPrefix { get; set; }
    public string PostBoxNumberSuffix { get; set; }
    public string PrimaryPremiseSuffix { get; set; }
    public string PrintPostZone { get; set; }
}