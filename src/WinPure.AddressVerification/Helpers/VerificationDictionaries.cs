namespace WinPure.AddressVerification.Helpers;

internal static class VerificationDictionaries
{
    #region US Dictionary
    internal static readonly Dictionary<string, string> VerificationStatus = new()
    {
        {"V","Verified" },
        {"P","Partially Verified" },
        {"U","Unverified" },
        {"A","Ambiguous" },
        {"C","Conflict" },
        {"R","Reverted" }
    };

    internal static readonly Dictionary<string, string> PostProcessed = new()
    {
        {"5", "Post-Processed: Delivery Point (PostBox or SubBuilding)"},
        {"4", "Post-Processed: Premise (Premise or Building)"},
        {"3", "Post-Processed: Thoroughfare"},
        {"2", "Post-Processed: Locality"},
        {"1", "Post-Processed: AdministrativeArea"},
        {"0", "Post-Processed: None"}
    };

    internal static readonly Dictionary<string, string> PreProcessed = new()
    {
        {"5", "Pre-Processed: Delivery Point (PostBox or SubBuilding)"},
        {"4", "Pre-Processed: Premise (Premise or Building)"},
        {"3", "Pre-Processed: Thoroughfare"},
        {"2", "Pre-Processed: Locality"},
        {"1", "Pre-Processed: AdministrativeArea"},
        {"0", "Pre-Processed: None"}
    };

    internal static readonly Dictionary<string, string> ParsingStatus = new()
    {
        {"I", "Identified and Parsed"},
        {"U", "Unable to parse"},
    };

    internal static readonly Dictionary<string, string> Lexicon = new()
    {
        {"5", "Lixicon: Delivery Point (PostBox or SubBuilding)"},
        {"4", "Lixicon: Premise (Premise or Building)"},
        {"3", "Lixicon: Thoroughfare"},
        {"2", "Lixicon: Locality"},
        {"1", "Lixicon: AdministrativeArea"},
        {"0", "Lixicon: None"}
    };

    internal static readonly Dictionary<string, string> Context = new()
    {
        {"5", "Context: Delivery Point (PostBox or SubBuilding)"},
        {"4", "Context: Premise (Premise or Building)"},
        {"3", "Context: Thoroughfare"},
        {"2", "Context: Locality"},
        {"1", "Context: AdministrativeArea"},
        {"0", "Context: None"}
    };

    internal static readonly Dictionary<string, string> PostcodeStatus = new()
    {
        {"P8", "PostalCodePrimary and PostalCodeSecondary verified"},
        {"P7", "PostalCodePrimary verified, PostalCodeSecondary added or changed"},
        {"P6", "PostalCodePrimary verified"},
        {"P5", "PostalCodePrimary verified with small change"},
        {"P4", "PostalCodePrimary verified with large change"},
        {"P3", "PostalCodePrimary added"},
        {"P2", "PostalCodePrimary identified by lexicon"},
        {"P1", "PostalCodePrimary identified by context"},
        {"P0", "PostalCodePrimary empty"}
    };

    internal static string GetIso3ByCountryName(string countryName)
    {
        switch (countryName)
        {
            case "Afghanistan": return "AFG";
            case "Albania": return "ALB";
            case "Algeria": return "DZA";
            case "American Samoa": return "ASM";
            case "Andorra": return "AND";
            case "Angola": return "AGO";
            case "Anguilla": return "AIA";
            case "Antarctica": return "ATA";
            case "Antigua and Barbuda": return "ATG";
            case "Argentina": return "ARG";
            case "Armenia": return "ARM";
            case "Aruba": return "ABW";
            case "Australia": return "AUS";
            case "Austria": return "AUT";
            case "Azerbaijan": return "AZE";
            case "Bahamas": return "BHS";
            case "Bahrain": return "BHR";
            case "Bangladesh": return "BGD";
            case "Barbados": return "BRB";
            case "Belarus": return "BLR";
            case "Belgium": return "BEL";
            case "Belize": return "BLZ";
            case "Benin": return "BEN";
            case "Bermuda": return "BMU";
            case "Bhutan": return "BTN";
            case "Bolivia, Plurinational State of": return "BOL";
            case "Bonaire, Sint Eustatius and Saba": return "BES";
            case "Bosnia and Herzegovina": return "BIH";
            case "Botswana": return "BWA";
            case "Brazil": return "BRA";
            case "British Indian Ocean Territory": return "IOT";
            case "Brunei Darussalam": return "BRN";
            case "Burkina Faso": return "BFA";
            case "Burundi": return "BDI";
            case "Cambodia": return "KHM";
            case "Cameroon": return "CMR";
            case "Canada": return "CAN";
            case "Cape Verde": return "CPV";
            case "Cayman Islands": return "CYM";
            case "Central African Republic": return "CAF";
            case "Chad": return "TCD";
            case "Chile": return "CHL";
            case "China": return "CHN";
            case "Christmas Island": return "CXR";
            case "Cocos (Keeling) Islands": return "CCK";
            case "Colombia": return "COL";
            case "Comoros": return "COM";
            case "Congo": return "COG";
            case "Congo, the Democratic Republic of the": return "COD";
            case "Cook Islands": return "COK";
            case "Costa Rica": return "CRI";
            case "Croatia": return "HRV";
            case "Cuba": return "CUB";
            case "Curaçao": return "CUW";
            case "Cyprus": return "CYP";
            case "Czech Republic": return "CZE";
            case "Côte d'Ivoire": return "CIV";
            case "Denmark": return "DNK";
            case "Djibouti": return "DJI";
            case "Dominica": return "DMA";
            case "Dominican Republic": return "DOM";
            case "Ecuador": return "ECU";
            case "Egypt": return "EGY";
            case "El Salvador": return "SLV";
            case "Equatorial Guinea": return "GNQ";
            case "Eritrea": return "ERI";
            case "Estonia": return "EST";
            case "Ethiopia": return "ETH";
            case "Falkland Islands (Malvinas)": return "FLK";
            case "Faroe Islands": return "FRO";
            case "Fiji": return "FJI";
            case "Finland": return "FIN";
            case "France": return "FRA";
            case "French Guiana": return "GUF";
            case "French Polynesia": return "PYF";
            case "French Southern Territories": return "ATF";
            case "Gabon": return "GAB";
            case "Gambia": return "GMB";
            case "Georgia": return "GEO";
            case "Germany": return "DEU";
            case "Ghana": return "GHA";
            case "Gibraltar": return "GIB";
            case "Greece": return "GRC";
            case "Greenland": return "GRL";
            case "Grenada": return "GRD";
            case "Guadeloupe": return "GLP";
            case "Guam": return "GUM";
            case "Guatemala": return "GTM";
            case "Guernsey": return "GGY";
            case "Guinea": return "GIN";
            case "Guinea-Bissau": return "GNB";
            case "Guyana": return "GUY";
            case "Haiti": return "HTI";
            case "Holy See (Vatican City State)": return "VAT";
            case "Honduras": return "HND";
            case "Hong Kong": return "HKG";
            case "Hungary": return "HUN";
            case "Iceland": return "ISL";
            case "India": return "IND";
            case "Indonesia": return "IDN";
            case "Iran, Islamic Republic of": return "IRN";
            case "Iraq": return "IRQ";
            case "Ireland": return "IRL";
            case "Isle of Man": return "IMN";
            case "Israel": return "ISR";
            case "Italy": return "ITA";
            case "Jamaica": return "JAM";
            case "Japan": return "JPN";
            case "Jersey": return "JEY";
            case "Jordan": return "JOR";
            case "Kazakhstan": return "KAZ";
            case "Kenya": return "KEN";
            case "Kiribati": return "KIR";
            case "Korea, Democratic People's Republic of": return "PRK";
            case "Korea, Republic of": return "KOR";
            case "Kuwait": return "KWT";
            case "Kyrgyzstan": return "KGZ";
            case "Lao People's Democratic Republic": return "LAO";
            case "Latvia": return "LVA";
            case "Lebanon": return "LBN";
            case "Lesotho": return "LSO";
            case "Liberia": return "LBR";
            case "Libya": return "LBY";
            case "Liechtenstein": return "LIE";
            case "Lithuania": return "LTU";
            case "Luxembourg": return "LUX";
            case "Macao": return "MAC";
            case "Macedonia, The Former Yugoslav Republic of": return "MKD";
            case "Madagascar": return "MDG";
            case "Malawi": return "MWI";
            case "Malaysia": return "MYS";
            case "Maldives": return "MDV";
            case "Mali": return "MLI";
            case "Malta": return "MLT";
            case "Marshall Islands": return "MHL";
            case "Martinique": return "MTQ";
            case "Mauritania": return "MRT";
            case "Mauritius": return "MUS";
            case "Mayotte": return "MYT";
            case "Mexico": return "MEX";
            case "Micronesia, Federated States of": return "FSM";
            case "Moldova, Republic of": return "MDA";
            case "Monaco": return "MCO";
            case "Mongolia": return "MNG";
            case "Montenegro": return "MNE";
            case "Montserrat": return "MSR";
            case "Morocco": return "MAR";
            case "Mozambique": return "MOZ";
            case "Myanmar": return "MMR";
            case "Namibia": return "NAM";
            case "Nauru": return "NRU";
            case "Nepal": return "NPL";
            case "Netherlands": return "NLD";
            case "Netherlands Antilles": return "ANT";
            case "New Caledonia": return "NCL";
            case "New Zealand": return "NZL";
            case "Nicaragua": return "NIC";
            case "Niger": return "NER";
            case "Nigeria": return "NGA";
            case "Niue": return "NIU";
            case "Norfolk Island": return "NFK";
            case "Northern Mariana Islands": return "MNP";
            case "Norway": return "NOR";
            case "Oman": return "OMN";
            case "Pakistan": return "PAK";
            case "Palau": return "PLW";
            case "Palestine, State of": return "PSE";
            case "Panama": return "PAN";
            case "Papua New Guinea": return "PNG";
            case "Paraguay": return "PRY";
            case "Peru": return "PER";
            case "Philippines": return "PHL";
            case "Pitcairn": return "PCN";
            case "Poland": return "POL";
            case "Portugal": return "PRT";
            case "Puerto Rico": return "PRI";
            case "Qatar": return "QAT";
            case "Republic of South Sudan": return "SSD";
            case "Romania": return "ROU";
            case "Russian Federation": return "RUS";
            case "Rwanda": return "RWA";
            case "Réunion": return "REU";
            case "Saint Barthélemy": return "BLM";
            case "Saint Helena, Ascension and Tristan da Cunha": return "SHN";
            case "Saint Kitts and Nevis": return "KNA";
            case "Saint Lucia": return "LCA";
            case "Saint Martin (French Part)": return "MAF";
            case "Saint Pierre and Miquelon": return "SPM";
            case "Saint Vincent and the Grenadines": return "VCT";
            case "Samoa": return "WSM";
            case "San Marino": return "SMR";
            case "Sao Tome and Principe": return "STP";
            case "Saudi Arabia": return "SAU";
            case "Senegal": return "SEN";
            case "Serbia": return "SRB";
            case "Seychelles": return "SYC";
            case "Sierra Leone": return "SLE";
            case "Singapore": return "SGP";
            case "Sint Maarten": return "SXM";
            case "Slovakia": return "SVK";
            case "Slovenia": return "SVN";
            case "Solomon Islands": return "SLB";
            case "Somalia": return "SOM";
            case "South Africa": return "ZAF";
            case "South Georgia and the South Sandwich Islands": return "SGS";
            case "Spain": return "ESP";
            case "Sri Lanka": return "LKA";
            case "Sudan": return "SDN";
            case "Suriname": return "SUR";
            case "Svalbard and Jan Mayen": return "SJM";
            case "Swaziland": return "SWZ";
            case "Sweden": return "SWE";
            case "Switzerland": return "CHE";
            case "Syrian Arab Republic": return "SYR";
            case "Taiwan, Province of China": return "TWN";
            case "Tajikistan": return "TJK";
            case "Tanzania, United Republic of": return "TZA";
            case "Thailand": return "THA";
            case "Timor-Leste": return "TLP";
            case "Togo": return "TGO";
            case "Tokelau": return "TKL";
            case "Tonga": return "TON";
            case "Trinidad and Tobago": return "TTO";
            case "Tunisia": return "TUN";
            case "Turkey": return "TUR";
            case "Turkmenistan": return "TKM";
            case "Turks and Caicos Islands": return "TCA";
            case "Tuvalu": return "TUV";
            case "Uganda": return "UGA";
            case "Ukraine": return "UKR";
            case "United Arab Emirates": return "ARE";
            case "United Kingdom": return "GBR";
            case "United States":
            case "USA":
            case "US": return "USA";
            case "United States Minor Outlying Islands": return "UMI";
            case "Uruguay": return "URY";
            case "Uzbekistan": return "UZB";
            case "Vanuatu": return "VUT";
            case "Venezuela, Bolivarian Republic": return "VEN";
            case "Viet Nam": return "VNM";
            case "Virgin Islands, British": return "VGB";
            case "Virgin Islands, U.S.": return "VIR";
            case "Wallis and Futuna": return "WLF";
            case "Western Sahara": return "ESH";
            case "Yemen": return "YEM";
            case "Zambia": return "ZMB";
            case "Zimbabwe": return "ZWE";
        }

        return countryName;
    }


    internal static string GetIso2ByCountryName(string countryName)
    {
        switch (countryName)
        {
            case "Afghanistan": return "AF";
            case "Albania": return "AL";
            case "Algeria": return "DZ";
            case "American Samoa": return "AS";
            case "Andorra": return "AD";
            case "Angola": return "AO";
            case "Anguilla": return "AI";
            case "Antarctica": return "AQ";
            case "Antigua and Barbuda": return "AG";
            case "Argentina": return "AR";
            case "Armenia": return "AM";
            case "Aruba": return "AW";
            case "Australia": return "AU";
            case "Austria": return "AT";
            case "Azerbaijan": return "AZ";
            case "Bahamas (the)": return "BS";
            case "Bahrain": return "BH";
            case "Bangladesh": return "BD";
            case "Barbados": return "BB";
            case "Belarus": return "BY";
            case "Belgium": return "BE";
            case "Belize": return "BZ";
            case "Benin": return "BJ";
            case "Bermuda": return "BM";
            case "Bhutan": return "BT";
            case "Bolivia (Plurinational State of)": return "BO";
            case "Bonaire, Sint Eustatius and Saba": return "BQ";
            case "Bosnia and Herzegovina": return "BA";
            case "Botswana": return "BW";
            case "Bouvet Island": return "BV";
            case "Brazil": return "BR";
            case "British Indian Ocean Territory (the)": return "IO";
            case "Brunei Darussalam": return "BN";
            case "Bulgaria": return "BG";
            case "Burkina Faso": return "BF";
            case "Burundi": return "BI";
            case "Cabo Verde": return "CV";
            case "Cambodia": return "KH";
            case "Cameroon": return "CM";
            case "Canada": return "CA";
            case "Cayman Islands (the)": return "KY";
            case "Central African Republic (the)": return "CF";
            case "Chad": return "TD";
            case "Chile": return "CL";
            case "China": return "CN";
            case "Christmas Island": return "CX";
            case "Cocos (Keeling) Islands (the)": return "CC";
            case "Colombia": return "CO";
            case "Comoros (the)": return "KM";
            case "Congo (the Democratic Republic of the)": return "CD";
            case "Congo (the)": return "CG";
            case "Cook Islands (the)": return "CK";
            case "Costa Rica": return "CR";
            case "Croatia": return "HR";
            case "Cuba": return "CU";
            case "Curaçao": return "CW";
            case "Cyprus": return "CY";
            case "Czechia": return "CZ";
            case "Côte d'Ivoire": return "CI";
            case "Denmark": return "DK";
            case "Djibouti": return "DJ";
            case "Dominica": return "DM";
            case "Dominican Republic (the)": return "DO";
            case "Ecuador": return "EC";
            case "Egypt": return "EG";
            case "El Salvador": return "SV";
            case "Equatorial Guinea": return "GQ";
            case "Eritrea": return "ER";
            case "Estonia": return "EE";
            case "Eswatini": return "SZ";
            case "Ethiopia": return "ET";
            case "Falkland Islands (the) [Malvinas]": return "FK";
            case "Faroe Islands (the)": return "FO";
            case "Fiji": return "FJ";
            case "Finland": return "FI";
            case "France": return "FR";
            case "French Guiana": return "GF";
            case "French Polynesia": return "PF";
            case "French Southern Territories (the)": return "TF";
            case "Gabon": return "GA";
            case "Gambia (the)": return "GM";
            case "Georgia": return "GE";
            case "Germany": return "DE";
            case "Ghana": return "GH";
            case "Gibraltar": return "GI";
            case "Greece": return "GR";
            case "Greenland": return "GL";
            case "Grenada": return "GD";
            case "Guadeloupe": return "GP";
            case "Guam": return "GU";
            case "Guatemala": return "GT";
            case "Guernsey": return "GG";
            case "Guinea": return "GN";
            case "Guinea-Bissau": return "GW";
            case "Guyana": return "GY";
            case "Haiti": return "HT";
            case "Heard Island and McDonald Islands": return "HM";
            case "Holy See (the)": return "VA";
            case "Honduras": return "HN";
            case "Hong Kong": return "HK";
            case "Hungary": return "HU";
            case "Iceland": return "IS";
            case "India": return "IN";
            case "Indonesia": return "ID";
            case "Iran (Islamic Republic of)": return "IR";
            case "Iraq": return "IQ";
            case "Ireland": return "IE";
            case "Isle of Man": return "IM";
            case "Israel": return "IL";
            case "Italy": return "IT";
            case "Jamaica": return "JM";
            case "Japan": return "JP";
            case "Jersey": return "JE";
            case "Jordan": return "JO";
            case "Kazakhstan": return "KZ";
            case "Kenya": return "KE";
            case "Kiribati": return "KI";
            case "Korea (the Democratic People's Republic of)": return "KP";
            case "Korea (the Republic of)": return "KR";
            case "Kuwait": return "KW";
            case "Kyrgyzstan": return "KG";
            case "Lao People's Democratic Republic(the)": return "LA";
            case "Latvia": return "LV";
            case "Lebanon": return "LB";
            case "Lesotho": return "LS";
            case "Liberia": return "LR";
            case "Libya": return "LY";
            case "Liechtenstein": return "LI";
            case "Lithuania": return "LT";
            case "Luxembourg": return "LU";
            case "Macao": return "MO";
            case "Madagascar": return "MG";
            case "Malawi": return "MW";
            case "Malaysia": return "MY";
            case "Maldives": return "MV";
            case "Mali": return "ML";
            case "Malta": return "MT";
            case "Marshall Islands (the)": return "MH";
            case "Martinique": return "MQ";
            case "Mauritania": return "MR";
            case "Mauritius": return "MU";
            case "Mayotte": return "YT";
            case "Mexico": return "MX";
            case "Micronesia (Federated States of)": return "FM";
            case "Moldova (the Republic of)": return "MD";
            case "Monaco": return "MC";
            case "Mongolia": return "MN";
            case "Montenegro": return "ME";
            case "Montserrat": return "MS";
            case "Morocco": return "MA";
            case "Mozambique": return "MZ";
            case "Myanmar": return "MM";
            case "Namibia": return "NA";
            case "Nauru": return "NR";
            case "Nepal": return "NP";
            case "Netherlands (the)": return "NL";
            case "New Caledonia": return "NC";
            case "New Zealand": return "NZ";
            case "Nicaragua": return "NI";
            case "Niger (the)": return "NE";
            case "Nigeria": return "NG";
            case "Niue": return "NU";
            case "Norfolk Island": return "NF";
            case "Northern Mariana Islands (the)": return "MP";
            case "Norway": return "NO";
            case "Oman": return "OM";
            case "Pakistan": return "PK";
            case "Palau": return "PW";
            case "Palestine, State of": return "PS";
            case "Panama": return "PA";
            case "Papua New Guinea": return "PG";
            case "Paraguay": return "PY";
            case "Peru": return "PE";
            case "Philippines (the)": return "PH";
            case "Pitcairn": return "PN";
            case "Poland": return "PL";
            case "Portugal": return "PT";
            case "Puerto Rico": return "PR";
            case "Qatar": return "QA";
            case "Republic of North Macedonia": return "MK";
            case "Romania": return "RO";
            case "Russian Federation (the)": return "RU";
            case "Rwanda": return "RW";
            case "Réunion": return "RE";
            case "Saint Barthélemy": return "BL";
            case "Saint Helena, Ascension and Tristan da Cunha": return "SH";
            case "Saint Kitts and Nevis": return "KN";
            case "Saint Lucia": return "LC";
            case "Saint Martin (French part)": return "MF";
            case "Saint Pierre and Miquelon": return "PM";
            case "Saint Vincent and the Grenadines": return "VC";
            case "Samoa": return "WS";
            case "San Marino": return "SM";
            case "Sao Tome and Principe": return "ST";
            case "Saudi Arabia": return "SA";
            case "Senegal": return "SN";
            case "Serbia": return "RS";
            case "Seychelles": return "SC";
            case "Sierra Leone": return "SL";
            case "Singapore": return "SG";
            case "Sint Maarten (Dutch part)": return "SX";
            case "Slovakia": return "SK";
            case "Slovenia": return "SI";
            case "Solomon Islands": return "SB";
            case "Somalia": return "SO";
            case "South Africa": return "ZA";
            case "South Georgia and the South Sandwich Islands": return "GS";
            case "South Sudan": return "SS";
            case "Spain": return "ES";
            case "Sri Lanka": return "LK";
            case "Sudan (the)": return "SD";
            case "Suriname": return "SR";
            case "Svalbard and Jan Mayen": return "SJ";
            case "Sweden": return "SE";
            case "Switzerland": return "CH";
            case "Syrian Arab Republic": return "SY";
            case "Taiwan (Province of China)": return "TW";
            case "Tajikistan": return "TJ";
            case "Tanzania, United Republic of": return "TZ";
            case "Thailand": return "TH";
            case "Timor-Leste": return "TL";
            case "Togo": return "TG";
            case "Tokelau": return "TK";
            case "Tonga": return "TO";
            case "Trinidad and Tobago": return "TT";
            case "Tunisia": return "TN";
            case "Turkey": return "TR";
            case "Turkmenistan": return "TM";
            case "Turks and Caicos Islands (the)": return "TC";
            case "Tuvalu": return "TV";
            case "Uganda": return "UG";
            case "Ukraine": return "UA";
            case "United Arab Emirates (the)": return "AE";
            case "United Kingdom of Great Britain and Northern Ireland (the)": return "GB";
            case "United States Minor Outlying Islands (the)": return "UM";
            case "United States of America (the)": return "US";
            case "Uruguay": return "UY";
            case "Uzbekistan": return "UZ";
            case "Vanuatu": return "VU";
            case "Venezuela (Bolivarian Republic of)": return "VE";
            case "Viet Nam": return "VN";
            case "Virgin Islands (British)": return "VG";
            case "Virgin Islands (U.S.)": return "VI";
            case "Wallis and Futuna": return "WF";
            case "Western Sahara": return "EH";
            case "Yemen": return "YE";
            case "Zambia": return "ZM";
            case "Zimbabwe": return "ZW";
            case "Åland Islands": return "AX";
        }

        return countryName;
    }

    internal static string GetIso2ByIso3(string countryCode)
    {
        switch (countryCode)
        {
            case "AFG": return "AF";
            case "ALB": return "AL";
            case "DZA": return "DZ";
            case "ASM": return "AS";
            case "AND": return "AD";
            case "AGO": return "AO";
            case "AIA": return "AI";
            case "ATA": return "AQ";
            case "ATG": return "AG";
            case "ARG": return "AR";
            case "ARM": return "AM";
            case "ABW": return "AW";
            case "AUS": return "AU";
            case "AUT": return "AT";
            case "AZE": return "AZ";
            case "BHS": return "BS";
            case "BHR": return "BH";
            case "BGD": return "BD";
            case "BRB": return "BB";
            case "BLR": return "BY";
            case "BEL": return "BE";
            case "BLZ": return "BZ";
            case "BEN": return "BJ";
            case "BMU": return "BM";
            case "BTN": return "BT";
            case "BOL": return "BO";
            case "BES": return "BQ";
            case "BIH": return "BA";
            case "BWA": return "BW";
            case "BVT": return "BV";
            case "BRA": return "BR";
            case "IOT": return "IO";
            case "BRN": return "BN";
            case "BGR": return "BG";
            case "BFA": return "BF";
            case "BDI": return "BI";
            case "CPV": return "CV";
            case "KHM": return "KH";
            case "CMR": return "CM";
            case "CAN": return "CA";
            case "CYM": return "KY";
            case "CAF": return "CF";
            case "TCD": return "TD";
            case "CHL": return "CL";
            case "CHN": return "CN";
            case "CXR": return "CX";
            case "CCK": return "CC";
            case "COL": return "CO";
            case "COM": return "KM";
            case "COD": return "CD";
            case "COG": return "CG";
            case "COK": return "CK";
            case "CRI": return "CR";
            case "HRV": return "HR";
            case "CUB": return "CU";
            case "CUW": return "CW";
            case "CYP": return "CY";
            case "CZE": return "CZ";
            case "CIV": return "CI";
            case "DNK": return "DK";
            case "DJI": return "DJ";
            case "DMA": return "DM";
            case "DOM": return "DO";
            case "ECU": return "EC";
            case "EGY": return "EG";
            case "SLV": return "SV";
            case "GNQ": return "GQ";
            case "ERI": return "ER";
            case "EST": return "EE";
            case "SWZ": return "SZ";
            case "ETH": return "ET";
            case "FLK": return "FK";
            case "FRO": return "FO";
            case "FJI": return "FJ";
            case "FIN": return "FI";
            case "FRA": return "FR";
            case "GUF": return "GF";
            case "PYF": return "PF";
            case "ATF": return "TF";
            case "GAB": return "GA";
            case "GMB": return "GM";
            case "GEO": return "GE";
            case "DEU": return "DE";
            case "GHA": return "GH";
            case "GIB": return "GI";
            case "GRC": return "GR";
            case "GRL": return "GL";
            case "GRD": return "GD";
            case "GLP": return "GP";
            case "GUM": return "GU";
            case "GTM": return "GT";
            case "GGY": return "GG";
            case "GIN": return "GN";
            case "GNB": return "GW";
            case "GUY": return "GY";
            case "HTI": return "HT";
            case "HMD": return "HM";
            case "VAT": return "VA";
            case "HND": return "HN";
            case "HKG": return "HK";
            case "HUN": return "HU";
            case "ISL": return "IS";
            case "IND": return "IN";
            case "IDN": return "ID";
            case "IRN": return "IR";
            case "IRQ": return "IQ";
            case "IRL": return "IE";
            case "IMN": return "IM";
            case "ISR": return "IL";
            case "ITA": return "IT";
            case "JAM": return "JM";
            case "JPN": return "JP";
            case "JEY": return "JE";
            case "JOR": return "JO";
            case "KAZ": return "KZ";
            case "KEN": return "KE";
            case "KIR": return "KI";
            case "PRK": return "KP";
            case "KOR": return "KR";
            case "KWT": return "KW";
            case "KGZ": return "KG";
            case "LAO": return "LA";
            case "LVA": return "LV";
            case "LBN": return "LB";
            case "LSO": return "LS";
            case "LBR": return "LR";
            case "LBY": return "LY";
            case "LIE": return "LI";
            case "LTU": return "LT";
            case "LUX": return "LU";
            case "MAC": return "MO";
            case "MDG": return "MG";
            case "MWI": return "MW";
            case "MYS": return "MY";
            case "MDV": return "MV";
            case "MLI": return "ML";
            case "MLT": return "MT";
            case "MHL": return "MH";
            case "MTQ": return "MQ";
            case "MRT": return "MR";
            case "MUS": return "MU";
            case "MYT": return "YT";
            case "MEX": return "MX";
            case "FSM": return "FM";
            case "MDA": return "MD";
            case "MCO": return "MC";
            case "MNG": return "MN";
            case "MNE": return "ME";
            case "MSR": return "MS";
            case "MAR": return "MA";
            case "MOZ": return "MZ";
            case "MMR": return "MM";
            case "NAM": return "NA";
            case "NRU": return "NR";
            case "NPL": return "NP";
            case "NLD": return "NL";
            case "NCL": return "NC";
            case "NZL": return "NZ";
            case "NIC": return "NI";
            case "NER": return "NE";
            case "NGA": return "NG";
            case "NIU": return "NU";
            case "NFK": return "NF";
            case "MNP": return "MP";
            case "NOR": return "NO";
            case "OMN": return "OM";
            case "PAK": return "PK";
            case "PLW": return "PW";
            case "PSE": return "PS";
            case "PAN": return "PA";
            case "PNG": return "PG";
            case "PRY": return "PY";
            case "PER": return "PE";
            case "PHL": return "PH";
            case "PCN": return "PN";
            case "POL": return "PL";
            case "PRT": return "PT";
            case "PRI": return "PR";
            case "QAT": return "QA";
            case "MKD": return "MK";
            case "ROU": return "RO";
            case "RUS": return "RU";
            case "RWA": return "RW";
            case "REU": return "RE";
            case "BLM": return "BL";
            case "SHN": return "SH";
            case "KNA": return "KN";
            case "LCA": return "LC";
            case "MAF": return "MF";
            case "SPM": return "PM";
            case "VCT": return "VC";
            case "WSM": return "WS";
            case "SMR": return "SM";
            case "STP": return "ST";
            case "SAU": return "SA";
            case "SEN": return "SN";
            case "SRB": return "RS";
            case "SYC": return "SC";
            case "SLE": return "SL";
            case "SGP": return "SG";
            case "SXM": return "SX";
            case "SVK": return "SK";
            case "SVN": return "SI";
            case "SLB": return "SB";
            case "SOM": return "SO";
            case "ZAF": return "ZA";
            case "SGS": return "GS";
            case "SSD": return "SS";
            case "ESP": return "ES";
            case "LKA": return "LK";
            case "SDN": return "SD";
            case "SUR": return "SR";
            case "SJM": return "SJ";
            case "SWE": return "SE";
            case "CHE": return "CH";
            case "SYR": return "SY";
            case "TWN": return "TW";
            case "TJK": return "TJ";
            case "TZA": return "TZ";
            case "THA": return "TH";
            case "TLS": return "TL";
            case "TGO": return "TG";
            case "TKL": return "TK";
            case "TON": return "TO";
            case "TTO": return "TT";
            case "TUN": return "TN";
            case "TUR": return "TR";
            case "TKM": return "TM";
            case "TCA": return "TC";
            case "TUV": return "TV";
            case "UGA": return "UG";
            case "UKR": return "UA";
            case "ARE": return "AE";
            case "GBR": return "GB";
            case "UMI": return "UM";
            case "USA": return "US";
            case "URY": return "UY";
            case "UZB": return "UZ";
            case "VUT": return "VU";
            case "VEN": return "VE";
            case "VNM": return "VN";
            case "VGB": return "VG";
            case "VIR": return "VI";
            case "WLF": return "WF";
            case "ESH": return "EH";
            case "YEM": return "YE";
            case "ZMB": return "ZM";
            case "ZWE": return "ZW";
            case "ALA": return "AX";
        }

        return countryCode;
    }
    #endregion
}