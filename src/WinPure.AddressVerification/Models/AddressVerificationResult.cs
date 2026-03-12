namespace WinPure.AddressVerification.Models;

internal class AddressVerificationResult : AddressVerificationData
{
    public string OrganizationName { get; set; }
    public string Address2 { get; set; }
    public string SubBuildingName { get; set; }
    public string PremiseNumber { get; set; }
    public string BuildingName { get; set; }
    public string Postbox { get; set; }
    public string DependentThoroughfareName { get; set; }
    public string ThoroughfareName { get; set; }
    public string DoubleDependentLocality { get; set; }
    public string DependentLocality { get; set; }
    public string SubAdministrativeArea { get; set; }
    public string SuperAdministrativeArea { get; set; }
    public string PostalCodePrimary { get; set; }
    public string PostalCodeSecondary { get; set; }
    public string DeliveryAddress { get; set; }

    public string Unmatched { get; set; }
    public string AddressQualityIndex { get; set; }
    public string AddressVerificationCode { get; set; }
    public string MatchScore { get; set; }

    // public string CassAutozoneIndicator { get; set; }
    public string CassCarrierRoute { get; set; }
    public string CassCmraIndicator { get; set; }
    public string CassCongressionalDistrict { get; set; }
    public string CassDefaultFlag { get; set; }
    public string CassDeliveryPointBarCode { get; set; }
    public string CassDpvConfirmedIndicator { get; set; }
    public string CassDpvFootnotes { get; set; }
    public string CassElotCode { get; set; }
    public string CassElotNumber { get; set; }
    public string CassEwsFlag { get; set; }
    public string CassFalsePositiveIndicator { get; set; }
    public string CassFipsCountyCode { get; set; }
    //  public string CassFootnotes { get; set; }
    public string CassLacsLinkCode { get; set; }
    public string CassLacsLinkIndicator { get; set; }
    public string CassLacsStatus { get; set; }
    public string CassNostatIndicator { get; set; }
    // public string CassPmbNumber { get; set; }
    // public string CassPmbType { get; set; }
    public string CassPrimaryAddressLine { get; set; }
    public string CassRecordType { get; set; }
    public string CassReturnCode { get; set; }
    public string CassResidentialDelivery { get; set; }
    public string CassSecondaryAddressLine { get; set; }
    public string CassSuiteLinkFootnote { get; set; }
    public string CassVacantIndicator { get; set; }

    public string GeoLatitude { get; set; }
    public string GeoLongitude { get; set; }
    public string GeoAccuracy { get; set; }
    public string GeoDistance { get; set; }

    public string AmasDpid { get; set; }
    public string AmasFloorType { get; set; }
    public string AmasFloorNumber { get; set; }
    public string AmasLotNumber { get; set; }
    public string AmasPostboxNumber { get; set; }
    public string AmasPostboxNumberPrefix { get; set; }
    public string AmasPostboxNumberSuffix { get; set; }
    public string AmasPrimaryPremise { get; set; }
    public string AmasPrimaryPremiseSuffix { get; set; }
    public string AmasSecondaryPremise { get; set; }
    public string AmasSecondaryPremiseSuffix { get; set; }
    public string AmasPresortZone { get; set; }
    public string AmasPrintPostZone { get; set; }
    public string AmasBarcode { get; set; }
    public string AmasPrimaryAddressLine { get; set; }
    public string AmasSecondaryAddressLine { get; set; }

    public string SerpStatusEx { get; set; }
    public string SerpQuestionable { get; set; }
    public string SerpResult { get; set; }
}