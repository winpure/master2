namespace WinPure.Common.Helpers
{
    public static class WinPureColumnNamesHelper
    {
        public const string WPCOLUMN_ALLVALUES_SUFFIX = "-AllValues";
        public const string WPCOLUMN_INCLUDE_IN_RESULT = "Include in Results";

        public const string WPCOLUMN_PRIMARY_KEY = "WinPurePrimK";
        public const string WPCOLUMN_POSSIBLEDUPLICATEID = "Possible duplicate ID";
        public const string WPCOLUMN_POSSIBLERELATEDID = "Possible related ID";
        public const string WPCOLUMN_POSSIBLEDUPLICATEID_KEY = "Possible duplicate key";
        public const string WPCOLUMN_POSSIBLERELATEDID_KEY = "Possible related key";
        public const string WPCOLUMN_POSSIBILITYID = "PossibilityGroupId";
        public const string WPCOLUMN_POSSIBILITYTYPE = "PossibilityType";
        public const string WPCOLUMN_POSSIBILITYKEY = "PossibilityKey";
        public const string WPCOLUMN_POSSIBILITYENTITYID = "PossibilityEntityId";

        public const string WPCOLUMN_GROUPID = "Group ID";
        public const string WPCOLUMN_SOURCENAME = "Source name";
        public const string WPCOLUMN_ISMASTER = "Is Master record";
        public const string WPCOLUMN_ISSELECTED = "Is Selected";
        public const string WPCOLUMN_TOTALSCORE = "Total Score";
        public const string WPCOLUMN_CHECK_ADDRESS_VERIFICATION = "WpCheckColumnForVerification";

        public const string WPCOLUMN_ORIGINAL_KEY = "OriginalKeyField";
        public const string WPCOLUMN_MATCH_KEY = "MatchKey";

        internal static string GetColumnGroupScoreName(int groupId) => $"Rule {groupId} Score";
        internal static string GetColumnConditionScoreName(int groupId, string conditionColumnName) => $"Rule {groupId} {conditionColumnName} Score";

        public const string WPCOLUMN_SPLIT_REGEX = "_Split_Regex";
        public const string WPCOLUMN_SPLIT_INTO_WORDS = "_Split_IntoWords";
        public const string WPCOLUMN_SPLIT_DOMAIN = "_Split_Domain";
        public const string WPCOLUMN_SPLIT_SUB_DOMAIN = "_Split_SubDomain";
        public const string WPCOLUMN_SPLIT_ACCOUNT = "_Split_Account";
        public const string WPCOLUMN_SPLIT_EMAIL_COUNTRY = "_Split_EmailCountry";
        public const string WPCOLUMN_SPLIT_EMAIL = "_Split_Email";
        public const string WPCOLUMN_SPLIT_EMAIL_NAME = "_Split_EmailName";

        public const string WPCOLUMN_SPLIT_DATE_YEAR = "_SplitDate_Year";
        public const string WPCOLUMN_SPLIT_DATE_MONTH = "_SplitDate_Month";
        public const string WPCOLUMN_SPLIT_DATE_DAY = "_SplitDate_Day";
        public const string WPCOLUMN_SPLIT_DATE_HOUR = "_SplitDate_Hour";
        public const string WPCOLUMN_SPLIT_DATE_MINUTE = "_SplitDate_Minute";
        public const string WPCOLUMN_SPLIT_DATE_SECOND = "_SplitDate_Second";
        public const string WPCOLUMN_SPLIT_PHONE_COUNTRY = "_SplitPhone_Country";
        public const string WPCOLUMN_SPLIT_PHONE_NUMBER = "_SplitPhone_Number";

        public const string WPCOLUMN_SPLIT_ADDRESS_PREFIX = "AP";
        public const string WPCOLUMN_SPLIT_ADDRESS_COUNTRY = "_Country";
        public const string WPCOLUMN_SPLIT_ADDRESS_STREETNAME = "_Street_Name";
        public const string WPCOLUMN_SPLIT_ADDRESS_CITY = "_City";
        public const string WPCOLUMN_SPLIT_ADDRESS_STATE = "_State";
        public const string WPCOLUMN_SPLIT_ADDRESS_POSTCODE = "_Postcode";
        public const string WPCOLUMN_SPLIT_ADDRESS_HOUSENUMBER = "_House_Number";
        public const string WPCOLUMN_SPLIT_ADDRESS_HOUSE = "_House";
        public const string WPCOLUMN_SPLIT_ADDRESS_POBOX = "_PO_Box";
        public const string WPCOLUMN_SPLIT_ADDRESS_UNIT = "_Unit";
        public const string WPCOLUMN_SPLIT_ADDRESS_STATEDISTRICT = "_State_District";
        public const string WPCOLUMN_SPLIT_ADDRESS_COUNTRYREGION = "_Country_Region";
        public const string WPCOLUMN_SPLIT_ADDRESS_WORLDREGION = "_World_Region";
        public const string WPCOLUMN_SPLIT_ADDRESS_CATEGORY = "_Category";
        public const string WPCOLUMN_SPLIT_ADDRESS_LEVEL = "_Level";
        public const string WPCOLUMN_SPLIT_ADDRESS_NEAR = "_Near";
        public const string WPCOLUMN_SPLIT_ADDRESS_STAIRCASE = "_Staircase";
        public const string WPCOLUMN_SPLIT_ADDRESS_ENTRANCE = "_ENTRANCE";
        public const string WPCOLUMN_SPLIT_ADDRESS_SUBURB = "_Suburb";
        public const string WPCOLUMN_SPLIT_ADDRESS_CITYDISTRICT = "_City_District";
        public const string WPCOLUMN_SPLIT_ADDRESS_ISLAND = "_Island";


        public const string WPCOLUMN_SPLIT_GENDER_PREFIX = "SplitName";
        public const string WPCOLUMN_SPLIT_NAME_PREFIX = "_Prefix";
        public const string WPCOLUMN_SPLIT_NAME_FIRST = "_First";
        public const string WPCOLUMN_SPLIT_NAME_MIDDLE = "_Middle";
        public const string WPCOLUMN_SPLIT_NAME_LAST = "_Last";
        public const string WPCOLUMN_SPLIT_NAME_SUFFIX = "_Suffix";
        public const string WPCOLUMN_SPLIT_NAME_QUALITY = "_Quality";
        public const string WPCOLUMN_SPLIT_NAME_GENDER = "_Gender";

        public const string WPCOLUMN_VALIDATE_EMAIL = "_IsValid";
        public const string WPCOLUMN_MERGE_RESULT = "MergeResult";

        public const string WPCOLUMN_USADDR_ORGANIZATIONNAME = "V_OrganizationName";
        public const string WPCOLUMN_USADDR_ADDRESS = "V_Address";
        public const string WPCOLUMN_USADDR_ADDRESS1 = "V_Address1";
        public const string WPCOLUMN_USADDR_ADDRESS2 = "V_Address2";
        public const string WPCOLUMN_USADDR_SUBBUILDINGNAME = "V_SubBuildingName";
        public const string WPCOLUMN_USADDR_PREMISENUMBER = "V_PremiseNumber";
        public const string WPCOLUMN_USADDR_BUILDINGNAME = "V_BuildingName";
        public const string WPCOLUMN_USADDR_POSTBOX = "V_PostBox";
        public const string WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME = "V_DependentThoroughfareName";
        public const string WPCOLUMN_USADDR_THOROUGHFARENAME = "V_ThoroughfareName";
        public const string WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY = "V_DoubleDependentLocality";
        public const string WPCOLUMN_USADDR_DEPENDENTLOCALITY = "V_DependentLocality";
        public const string WPCOLUMN_USADDR_LOCALITY = "V_Locality";
        public const string WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA = "V_SubAdministrativeArea";
        public const string WPCOLUMN_USADDR_ADMINISTRATIVEAREA = "V_AdministrativeArea";
        public const string WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA = "V_SuperAdministrativeArea";
        public const string WPCOLUMN_USADDR_POSTALCODE = "V_PostalCode";
        public const string WPCOLUMN_USADDR_POSTALCODEPRIMARY = "V_PostalCodePrimary";
        public const string WPCOLUMN_USADDR_POSTALCODESECONDARY = "V_PostalCodeSecondary";
        public const string WPCOLUMN_USADDR_DELIVERYADDRESS = "V_DeliveryAddress";
        public const string WPCOLUMN_USADDR_COUNTRY = "V_Country";

        public const string WPCOLUMN_USADDR_UNMATCHED = "V_Unmatched";
        public const string WPCOLUMN_USADDR_ADDRESSQUALITYINDEX = "V_AddressQualityIndex";
        public const string WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE = "V_AddressVerificationCode";
        public const string WPCOLUMN_USADDR_MATCHSCORE = "V_Matchscore";



        public const string WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR = "V_CASS_AutoZoneIndicator";
        public const string WPCOLUMN_USADDR_CASS_CARRIERROUTE = "V_CASS_CarrierRoute";
        public const string WPCOLUMN_USADDR_CASS_CMRAINDICATOR = "V_CASS_CMRAIndicator";
        public const string WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT = "V_CASS_CongressionalDistrict";
        public const string WPCOLUMN_USADDR_CASS_DEFAULTFLAG = "V_CASS_DefaultFlag";
        public const string WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE = "V_CASS_DeliveryPointBarCode";
        public const string WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR = "V_CASS_DPVConfirmedIndicator";
        public const string WPCOLUMN_USADDR_CASS_DPVFOOTNOTES = "V_CASS_DPVFootnotes";
        public const string WPCOLUMN_USADDR_CASS_ELOTCODE = "V_CASS_eLOTCode";
        public const string WPCOLUMN_USADDR_CASS_ELOTNUMBER = "V_CASS_eLOTNumber";
        public const string WPCOLUMN_USADDR_CASS_EWSFLAG = "V_CASS_EWSFlag";
        public const string WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR = "V_CASS_FalsePositiveIndicator";
        public const string WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE = "V_CASS_FIPSCountyCode";
       // public const string WPCOLUMN_USADDR_CASS_FOOTNOTES = "V_CASS_Footnotes";
        public const string WPCOLUMN_USADDR_CASS_LACSLINKCODE = "V_CASS_LACSLinkCode";
        public const string WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR = "V_CASS_LACSLinkIndicator";
        public const string WPCOLUMN_USADDR_CASS_LACSSTATUS = "V_CASS_LACSStatus";
        public const string WPCOLUMN_USADDR_CASS_NOSTATINDICATOR = "V_CASS_NoStatIndicator";
       // public const string WPCOLUMN_USADDR_CASS_PMBNUMBER = "V_CASS_PMBNumber";
       // public const string WPCOLUMN_USADDR_CASS_PMBTYPE = "V_CASS_PMBType";
        public const string WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE = "V_CASS_PrimaryAddressLine";
        public const string WPCOLUMN_USADDR_CASS_RECORDTYPE = "V_CASS_RecordType";
        public const string WPCOLUMN_USADDR_CASS_RETURNCODE = "V_CASS_ReturnCode";
        public const string WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY = "V_CASS_ResidentialDelivery";
       // public const string WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE = "V_CASS_SecondaryAddressLine";
        public const string WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE = "V_CASS_SUITELinkFootnote";
        public const string WPCOLUMN_USADDR_CASS_VACANTINDICATOR = "V_CASS_VacantIndicator";


        public const string WPCOLUMN_USADDR_GEO_LATITUDE = "V_GEO_Latitude";
        public const string WPCOLUMN_USADDR_GEO_LONGITUDE = "V_GEO_Longitude";
        public const string WPCOLUMN_USADDR_GEO_GEOACCURACY = "V_GEO_GeoAccuracy";
        public const string WPCOLUMN_USADDR_GEO_GEODISTANCE = "V_GEO_GeoDistance";


        public const string WPCOLUMN_USADDR_AMAS_DPID = "V_AMAS_DPID";
        public const string WPCOLUMN_USADDR_AMAS_FLOORTYPE = "V_AMAS_FloorType";
        public const string WPCOLUMN_USADDR_AMAS_FLOORNUMBER = "V_AMAS_FloorNumber";
        public const string WPCOLUMN_USADDR_AMAS_LOTNUMBER = "V_AMAS_LotNumber";
        public const string WPCOLUMN_USADDR_AMAS_POSTBOXNUM = "V_AMAS_PostBoxNum";
        public const string WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX = "V_AMAS_PostBoxNumberPrefix";
        public const string WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX = "V_AMAS_PostBoxNumberSuffix";
        public const string WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE = "V_AMAS_PrimaryPremise";
        public const string WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX = "V_AMAS_PrimaryPremiseSuffix";
        public const string WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE = "V_AMAS_SecondaryPremise";
        public const string WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX = "V_AMAS_SecondaryPremiseSuffix";
        public const string WPCOLUMN_USADDR_AMAS_PRESORTZONE = "V_AMAS_PreSortZone";
        public const string WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE = "V_AMAS_PrintPostZone";
        public const string WPCOLUMN_USADDR_AMAS_BARCODE = "V_AMAS_Barcode";
        public const string WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE = "V_AMAS_PrimaryAddressLine";
        public const string WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE = "V_AMAS_SecondaryAddressLine";


        public const string WPCOLUMN_USADDR_SERP_SERPSTATUSEX = "V_SERP_SerpStatusEx";
        public const string WPCOLUMN_USADDR_SERP_QUESTIONABLE = "V_SERP_Questionable";
        public const string WPCOLUMN_USADDR_SERP_RESULT = "V_SERP_Result";
    }
}