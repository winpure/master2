using System.Threading;
using Newtonsoft.Json;
using WinPure.Common.Abstractions;
using WinPure.Configuration.Models.Configuration;
using WinPure.Cleansing.Properties;
using WinPure.Common.Enums;
using WinPure.DataService.Senzing.Models;
using WinPure.Matching.Algorithm;
using WinPure.Matching.Enums;

namespace WinPure.DataService.DependencyInjection;

internal static partial class WinPureDataServiceDependencyExtension
{
    private class SenzingService : WinPureNotification, ISenzingService
    {
        private readonly IWpLogger _logger;
        private readonly IEntityResolutionMappingSettingService _erMappingService;
        private readonly ILicenseService _licenseService;

        private WinPureConfiguration _configuration;

        public SenzingService(IWpLogger logger,
            IConfigurationService configurationService,
            IEntityResolutionMappingSettingService erMappingService,
            ILicenseService licenseService)
        {
            _logger = logger;
            _erMappingService = erMappingService;
            _licenseService = licenseService;
            _configuration = configurationService.Configuration;
        }

        public List<FieldType> GetFieldTypes()
        {
            return JsonConvert.DeserializeObject<List<FieldType>>(SenzinTypesJson);
        }

        public EntityResolutionReport RunAnalyze(string dbPath, List<EntityResolutionConfiguration> configurations, CancellationToken cToken)
        {
            foreach (var configuration in configurations)
            {
                VerifyRowConfigurations(configuration.TableDisplayName, configuration.Source, configuration.Rows);

                var columnsToSelect = $"{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}";
                if (configuration.IsMainTable)
                {
                    columnsToSelect += $", \"{configuration.TableName}:\" || {WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY} AS \"TRUSTED_ID_NUMBER\", \"REFERENCE\" AS \"TRUSTED_ID_TYPE\"";
                }
                foreach (var rowConfiguration in configuration.Rows.Where(x => !x.IsIgnore))
                {
                    var columnName = configuration.Source == ExternalSourceTypes.JSONL ? rowConfiguration.ColumnName : SenzingHelper.GetSenzingFieldName(rowConfiguration);
                    var statement = $"\"{rowConfiguration.ColumnName}\" AS \"{columnName}\"";

                    columnsToSelect += ", " + statement;
                }

                configuration.Query = $"SELECT {columnsToSelect} FROM {configuration.TableName} ";
            }

            var erRecordLimit = _licenseService.IsDemo
                ? GlobalConstants.ErRecordsForDemoVersion
                : _licenseService.GetErRecordLimit();

            var pipeline = new SenzingPipeline(_configuration.ErSettings, dbPath, erRecordLimit, NotifyProgress, _logger);
            return pipeline.ExecutePipeline(configurations, cToken);
        }

        public List<MapError> VerifyRowConfigurations(string tableName, ExternalSourceTypes sourceType, List<EntityResolutionRowConfiguration> rows)
        {
            var errors = new List<MapError>();
            if (rows.All(x => x.IsIgnore || x.IsInclude))
            {
                errors.Add(new MapError
                { Message = $"Field map for the table {tableName} must contains at least one mapping" });
            }

            if (sourceType == ExternalSourceTypes.JSONL)
                return errors;

            var notMapped = rows.Where(x => string.IsNullOrWhiteSpace(x.FieldType) && !x.IsInclude && !x.IsIgnore)
                .ToList();
            foreach (var field in notMapped)
            {
                errors.Add(new MapError
                {
                    ColumnName = field.ColumnName,
                    Message = $"Field {field.ColumnName} must be mapped to any type, included or ignored"
                });
            }

            if (rows.Any(x => string.IsNullOrEmpty(x.FieldType) && x.IsInclude &&
                              (x.ColumnName.ToUpper() == "DATA_SOURCE" ||
                               x.ColumnName.ToUpper() == "RECORD_ID")))
            {
                errors.Add(new MapError
                {
                    Message =
                        "Please do not use reserved name 'DATA_SOURCE' and 'RECORD_ID' as field name." + Environment.NewLine + "Ignore those fields from entity resolution, rename them or map them to any other type."
                });
            }

            var duplicates = rows.Where(x => !x.IsIgnore && !x.IsInclude)
                .GroupBy(g => new { g.Label, g.FieldType })
                .Select(x => new
                {
                    x.Key.Label,
                    x.Key.FieldType,
                    Cnt = x.Count()
                }).Where(x => x.Cnt > 1).ToList();

            foreach (var duplicate in duplicates)
            {
                errors.Add(new MapError
                {
                    TypeName = duplicate.FieldType,
                    Message =
                        $"Field map for the table {tableName} must only contain unique mapping {duplicate.Label + " " + duplicate.FieldType}. Please use Label."
                });
            }

            var mapping = _erMappingService.GetAll();
            var conflicts = mapping.Where(x => x.ConflictEntityTypes.Any()).GroupBy(g => g.EntityType)
                .Select(x => new
                {
                    EntityType = x.Key.ToUpper(),
                    ConflictFields = x.SelectMany(s => s.ConflictEntityTypes).Distinct().Select(x => x.ToUpper())
                        .ToList()
                }).ToList();

            foreach (var conflict in conflicts)
            {
                var possibleConflictRow = rows.FirstOrDefault(x => x.FieldType.ToUpper() == conflict.EntityType);
                if (possibleConflictRow != null)
                {
                    var conflictRows = rows.Where(x =>
                        x.Label.ToUpper() == possibleConflictRow.Label.ToUpper() &&
                        conflict.ConflictFields.Contains(x.FieldType.ToUpper())).ToList();
                    if (conflictRows.Any())
                    {
                        errors.Add(new MapError
                        {
                            ColumnName = possibleConflictRow.ColumnName,
                            TypeName = possibleConflictRow.FieldType.ToUpper(),
                            Message =
                                $"Type {possibleConflictRow.FieldType} cannot be used with some other type(s) with the same label."
                        });

                        foreach (var conflictRow in conflictRows)
                        {
                            errors.Add(new MapError
                            {
                                ColumnName = conflictRow.ColumnName,
                                TypeName = conflictRow.FieldType.ToUpper(),
                                Message =
                                    $"Type {possibleConflictRow.FieldType} cannot be used with type {conflictRow.FieldType} with the same label."
                            });
                        }
                    }
                }
            }

            return errors;
        }

        public void MapColumns(List<EntityResolutionRowConfiguration> rows)
        {
            var mappingConfiguration = _erMappingService.GetAll();
            var fuzzyCompare = FuzzyFactory.GetFuzzyAlgorithm(MatchAlgorithm.WinPureFuzzy);
            var labels = mappingConfiguration.Where(x => !string.IsNullOrEmpty(x.UsageGroup)).Select(x => x.UsageGroup)
                .Union(new List<string> { "SECONDARY", "NATIVE" }).Distinct().ToList();
            var notMapped = mappingConfiguration.Where(x => string.IsNullOrEmpty(x.EntityType)).ToList();
            var senzigTypes = GetFieldTypes().Where(x => !string.IsNullOrWhiteSpace(x.SystemName)).ToList();
            var canMapped = mappingConfiguration
                .Where(x => !string.IsNullOrEmpty(x.EntityType) && !x.PrerequisiteEntityTypes.Any())
                .OrderBy(x => x.DataColumnName).ToList();
            var canMappedDirectly = canMapped.Where(x => x.ExactMatch).OrderBy(x => x.DataColumnName).ToList();
            //var canConflict = mappingConfiguration.Where(x => !string.IsNullOrEmpty(x.EntityType) && x.ConflictEntityTypes.Any()).ToList();
            var hasPrerequisite = mappingConfiguration
                .Where(x => !string.IsNullOrEmpty(x.EntityType) && x.PrerequisiteEntityTypes.Any()).ToList();

            //Exclude already mapped rows
            var notMappedRows = rows.Where(x => string.IsNullOrEmpty(x.FieldType)).ToList();

            var rowsForMapping = notMappedRows.Where(x =>
                !notMapped.Any(m => CompareTwoNames(m.DataColumnName, x.ColumnName, fuzzyCompare) > 0.99)).ToList();

            foreach (var row in rowsForMapping)
            {
                if (row.ColumnName.ToLower() == "account name")
                {
                    continue;
                }

                //exact match by entity type
                var mapExact = senzigTypes.FirstOrDefault(x =>
                    string.Equals(row.ColumnName, x.SystemName, StringComparison.InvariantCultureIgnoreCase));
                if (mapExact != null)
                {
                    row.FieldType = mapExact.SystemName;
                    row.IsInclude = false;
                    row.IsIgnore = false;
                    var label = labels.FirstOrDefault(x => row.ColumnName.ToUpper().Contains(x.ToUpper()));

                    row.Label = label ?? String.Empty;
                    continue;
                }

                //exact match by entity type
                var mapExactWithLabel = senzigTypes.FirstOrDefault(x => row.ColumnName.EndsWith(x.SystemName));
                if (mapExactWithLabel != null)
                {
                    var label = row.ColumnName.Replace(mapExactWithLabel.SystemName, "");

                    if (label.EndsWith("_"))
                        label = label.Substring(0, label.Length - 1);

                    row.FieldType = mapExactWithLabel.SystemName;
                    row.IsInclude = false;
                    row.IsIgnore = false;
                    row.Label = label;
                    continue;
                }

                //match by dictionary
                var possibleMap = canMappedDirectly.FirstOrDefault(x =>
                                      string.Equals(x.DataColumnName, row.ColumnName,
                                          StringComparison.InvariantCultureIgnoreCase)) ??
                                  canMapped.Select(x => new
                                  {
                                      tp = x,
                                      score = CompareTwoNames(x.DataColumnName, row.ColumnName, fuzzyCompare)
                                  })
                                      .Where(x => x.score > 0.97)
                                      .OrderByDescending(x => x.score).FirstOrDefault()?.tp;

                if (possibleMap != null)
                {
                    row.FieldType = possibleMap.EntityType;
                    row.Label = possibleMap.UsageGroup;
                    row.IsIgnore = false;
                    row.IsInclude = false;
                }
            }

            var rowsToUpdate = rowsForMapping.Where(x =>
                    hasPrerequisite.Any(m => CompareTwoNames(m.DataColumnName, x.ColumnName, fuzzyCompare) > 0.99))
                .ToList();
            foreach (var row in rowsToUpdate)
            {
                if (rowsForMapping.Any(x =>
                        x.ColumnName != row.ColumnName && x.FieldType == row.FieldType && x.Label == row.Label))
                {
                    var prerequisite = hasPrerequisite.FirstOrDefault(m =>
                        CompareTwoNames(m.DataColumnName, row.ColumnName, fuzzyCompare) > 0.99);
                    if (prerequisite != null)
                    {
                        row.FieldType = prerequisite.EntityType;
                        row.Label = prerequisite.UsageGroup;
                    }
                }
            }
        }

        /// <summary>
        /// Export clean settings to json file
        /// </summary>
        /// <param name="settings">WinPure clean settings</param>
        /// <param name="destinationPath">Destination folder</param>
        /// <param name="fileName">File name (without extension).</param>
        public void ExportErSettings(List<EntityResolutionRowConfiguration> settings, string fileName)
        {
            if (settings == null)
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_SETTINGSNOTNULL);
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_PATHANDFILE);
            }

            try
            {
                var fileContent = JsonConvert.SerializeObject(settings);
                FileHelper.CreateOrOverrideFile(fileName, fileContent);
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot export settings to file", exception);
                throw new WinPureCannotProcessFile(Resources.EXCEPTION_EXPORT_SETTING_ERROR, exception);
            }
        }

        /// <summary>
        /// Import WinPure clean settings from json file
        /// </summary>
        /// <param name="destinationFile">Json file with settings</param>
        /// <returns></returns>
        public List<EntityResolutionRowConfiguration> ImportErSettings(string destinationFile)
        {
            try
            {
                var content = FileHelper.ReadFile(destinationFile);
                var settings = JsonConvert.DeserializeObject<List<EntityResolutionRowConfiguration>>(content);
                return settings;
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot import settings to file", exception);
                throw new WinPureCannotProcessFile(Resources.EXCEPTION_IMPORT_SETTING_ERROR, exception);
            }
        }

        private double CompareTwoNames(string mappedColumn, string columnName, IFuzzyComparison fuzzy)
        {
            return mappedColumn.Length <= 3 || columnName.Length <= 3
                ? (string.Compare(mappedColumn, columnName, StringComparison.InvariantCultureIgnoreCase) == 0 ? 1 : 0)
                : fuzzy.CompareString(mappedColumn.ToUpper(), columnName.ToUpper());
        }

        private static string SenzinTypesJson =
    @"[{""Id"":1,""Name"":""Name"",""ParentId"":0},
    {""Id"":2,""Name"":""Addresses"",""ParentId"":0},
    {""Id"":3,""Name"":""Phone"",""ParentId"":0},
    {""Id"":4,""Name"":""Physical Attributes"",""ParentId"":0},
    {""Id"":5,""Name"":""Government Attributes"",""ParentId"":0},
    {""Id"":6,""Name"":""Organization Identifier"",""ParentId"":0},
    {""Id"":7,""Name"":""Identifier"",""ParentId"":0},
    {""Id"":8,""Name"":""Group association"",""ParentId"":0},
    {""Id"":84,""Name"":""Location"",""ParentId"":0},
    {""Id"":9,""Name"":""Name Type"",""ParentId"":1,""SystemName"":""NAME_TYPE"",""Description"":""Most data sources have only one name, but when there are multiple, there is usually one primary name and the rest are aliases.  Whatever terms are used here should be standardized across the data sources included in your project."",""Example"":""PRIMARY, ALIAS""},
    {""Id"":10,""Name"":""Name Full"",""ParentId"":1,""SystemName"":""NAME_FULL"",""Description"":""This is the full name of an individual.  It should only be populated when the parsed name of an individual is not available, although parsed names for an individual are most desirable.  The system will not allow both a full name and the parsed names to be populated in the same set of name fields. [See handling duplicate columns later in this document.]"",""Example"":""Robert J Smith""},
    {""Id"":11,""Name"":""Name Org"",""ParentId"":1,""SystemName"":""NAME_ORG"",""Description"":""This is the organization name."",""Example"":""Acme Tire Inc.""},
    {""Id"":12,""Name"":""Name Last"",""ParentId"":1,""SystemName"":""NAME_LAST"",""Description"":""This is the last or surname of an individual."",""Example"":""Smith""},
    {""Id"":13,""Name"":""Name First"",""ParentId"":1,""SystemName"":""NAME_FIRST"",""Description"":""This is the first or given name of an individual."",""Example"":""Robert""},
    {""Id"":14,""Name"":""Name Middle"",""ParentId"":1,""SystemName"":""NAME_MIDDLE"",""Description"":""This is the middle name of an individual."",""Example"":""J""},
    {""Id"":15,""Name"":""Name Prefix"",""ParentId"":1,""SystemName"":""NAME_PREFIX"",""Description"":""This is a prefix for an individual's name such as the titles: Mr, Mrs, Ms, Dr, etc.  "",""Example"":""Mr""},
    {""Id"":16,""Name"":""Name Suffix"",""ParentId"":1,""SystemName"":""NAME_SUFFIX"",""Description"":""This is a suffix for an individual's name and may include generational references such as: JR, SR, I, II, III and/or professional designations such as: MD, PHD, PMP, etc.  "",""Example"":""MD""},
    {""Id"":17,""Name"":""Addr Type"",""ParentId"":2,""SystemName"":""ADDR_TYPE"",""Description"":""This is a code that describes how the address is being used such as: HOME, MAILING, BUSINESS*, etc.  Whatever terms are used here should be standardized across the data sources included in your project."",""Example"":""HOME""},
    {""Id"":18,""Name"":""Addr Full"",""ParentId"":2,""SystemName"":""ADDR_FULL"",""Description"":""This is a single string containing the all address lines plus city, state, zip and country.  Sometimes data sources have this rather than parsed address.  Only populate this field if the parsed address lines are not available.""},
    {""Id"":19,""Name"":""Addr Line1"",""ParentId"":2,""SystemName"":""ADDR_LINE1"",""Description"":""This is the first address line and is required if an address is presented."",""Example"":""111 First St""},
    {""Id"":20,""Name"":""Addr Line2"",""ParentId"":2,""SystemName"":""ADDR_LINE2"",""Description"":""This is the second address line if needed."",""Example"":""Suite 101""},
    {""Id"":21,""Name"":""Addr Line3"",""ParentId"":2,""SystemName"":""ADDR_LINE3"",""Description"":""This is the third address line if needed.""},
    {""Id"":22,""Name"":""Addr Line4"",""ParentId"":2,""SystemName"":""ADDR_LINE4"",""Description"":""This is the fourth address line if needed.""},
    {""Id"":23,""Name"":""Addr Line5"",""ParentId"":2,""SystemName"":""ADDR_LINE5"",""Description"":""This is the fifth address line if needed.""},
    {""Id"":24,""Name"":""Addr Line6"",""ParentId"":2,""SystemName"":""ADDR_LINE6"",""Description"":""This is the sixth address line if needed.""},
    {""Id"":25,""Name"":""Addr City"",""ParentId"":2,""SystemName"":""ADDR_CITY"",""Description"":""This is the city of the address."",""Example"":""Las Vegas""},
    {""Id"":26,""Name"":""Addr State"",""ParentId"":2,""SystemName"":""ADDR_STATE"",""Description"":""This is the state or province of the address."",""Example"":""NV""},
    {""Id"":27,""Name"":""Addr Postal Code"",""ParentId"":2,""SystemName"":""ADDR_POSTAL_CODE"",""Description"":""This is the zip or postal code of the address."",""Example"":""89111""},
    {""Id"":28,""Name"":""Addr Country"",""ParentId"":2,""SystemName"":""ADDR_COUNTRY"",""Description"":""This is the country of the address."",""Example"":""US""},
    {""Id"":29,""Name"":""Addr From Date"",""ParentId"":2,""SystemName"":""ADDR_FROM_DATE"",""Description"":""This is the date the entity started using the address if known. It is the used to determine the latest value of this type being used by the entity."",""Example"":""2016-01-14""},
    {""Id"":30,""Name"":""Addr Thru Date"",""ParentId"":2,""SystemName"":""ADDR_THRU_DATE"",""Description"":""This is the date the entity stopped using the address if known.""},
    {""Id"":31,""Name"":""Phone Type"",""ParentId"":3,""SystemName"":""PHONE_TYPE"",""Description"":""This is a code that describes how the phone is being used such as: HOME, FAX, MOBILE*, etc.  Whatever terms are used here should be standardized across the data sources included in your project."",""Example"":""MOBILE""},
    {""Id"":32,""Name"":""Phone Number"",""ParentId"":3,""SystemName"":""PHONE_NUMBER"",""Description"":""This is the actual phone number."",""Example"":""111-11-1111""},
    {""Id"":33,""Name"":""Phone From Date"",""ParentId"":3,""SystemName"":""PHONE_FROM_DATE"",""Description"":""This is the date the entity started using the phone number if known.  It is the used to determine the latest value of this type being used by the entity."",""Example"":""2016-01-14""},
    {""Id"":34,""Name"":""Phone Thru Date"",""ParentId"":3,""SystemName"":""PHONE_THRU_DATE"",""Description"":""This is the date the entity stopped using the phone number if known.""},
    {""Id"":35,""Name"":""Gender"",""ParentId"":4,""SystemName"":""GENDER"",""Description"":""This is the gender such as M for Male and F for Female."",""Example"":""M""},
    {""Id"":36,""Name"":""Date Of Birth"",""ParentId"":4,""SystemName"":""DATE_OF_BIRTH"",""Description"":""This is the date of birth for a person and partial dates such as just month and day or just month and year are ok."",""Example"":""1980-05-14""},
    {""Id"":37,""Name"":""Date Of Death"",""ParentId"":4,""SystemName"":""DATE_OF_DEATH"",""Description"":""This is the date of death for a person.  Again, partial dates are ok."",""Example"":""2010-05-14""},
    {""Id"":38,""Name"":""Nationality"",""ParentId"":4,""SystemName"":""NATIONALITY"",""Description"":""This is where the person was born and should contain a country name or code"",""Example"":""US""},
    {""Id"":39,""Name"":""Citizenship"",""ParentId"":4,""SystemName"":""CITIZENSHIP"",""Description"":""This is the country the person is a citizen of and should contain a country name or code."",""Example"":""US""},
    {""Id"":40,""Name"":""Place Of Birth"",""ParentId"":4,""SystemName"":""PLACE_OF_BIRTH"",""Description"":""This is where the person was born.  Ideally it is a country name or code.  However, they often contain city names as well."",""Example"":""US""},
    {""Id"":41,""Name"":""Registration Date"",""ParentId"":4,""SystemName"":""REGISTRATION_DATE"",""Description"":""This is the date the organization was registered, like date of birth is to a person."",""Example"":""2010-05-14""},
    {""Id"":42,""Name"":""Registration Country"",""ParentId"":4,""SystemName"":""REGISTRATION_COUNTRY"",""Description"":""This is the country the organization was registered in, like place of birth is to a person."",""Example"":""US""},
    {""Id"":43,""Name"":""Passport Number"",""ParentId"":5,""SystemName"":""PASSPORT_NUMBER"",""Description"":""This is the passport number."",""Example"":""123456789""},
    {""Id"":44,""Name"":""Passport Country"",""ParentId"":5,""SystemName"":""PASSPORT_COUNTRY"",""Description"":""This is the country that issued the ID."",""Example"":""US""},
    {""Id"":45,""Name"":""Drivers License Number"",""ParentId"":5,""SystemName"":""DRIVERS_LICENSE_NUMBER"",""Description"":""This the driver's license number."",""Example"":""123456789""},
    {""Id"":46,""Name"":""Drivers License State"",""ParentId"":5,""SystemName"":""DRIVERS_LICENSE_STATE"",""Description"":""This is the state or province that issued the driver's license. "",""Example"":""NV""},
    {""Id"":47,""Name"":""Ssn Number"",""ParentId"":5,""SystemName"":""SSN_NUMBER"",""Description"":""This is the US Social Security number."",""Example"":""123-12-1234""},
    {""Id"":48,""Name"":""Ssn Last4"",""ParentId"":5,""SystemName"":""SSN_LAST4"",""Description"":""This is just the last4 digits of the SSN for use when the full SSN is not available."",""Example"":""1234""},
    {""Id"":49,""Name"":""National Id Number"",""ParentId"":5,""SystemName"":""NATIONAL_ID_NUMBER"",""Description"":""This is the national insurance number issued by many countries.  It is similar to an SSN in the US."",""Example"":""123121234""},
    {""Id"":50,""Name"":""National Id Country"",""ParentId"":5,""SystemName"":""NATIONAL_ID_COUNTRY"",""Description"":""This is the country that issued the ID."",""Example"":""CA""},
    {""Id"":51,""Name"":""Tax Id Type"",""ParentId"":5,""SystemName"":""TAX_ID_TYPE"",""Description"":""This is the tax id number for a company, as opposed to an SSN or NIN for an individual."",""Example"":""EIN""},
    {""Id"":52,""Name"":""Tax Id Number"",""ParentId"":5,""SystemName"":""TAX_ID_NUMBER"",""Description"":""This is the actual ID number."",""Example"":""123121234""},
    {""Id"":53,""Name"":""Tax Id Country"",""ParentId"":5,""SystemName"":""TAX_ID_COUNTRY"",""Description"":""This is the country that issued the ID."",""Example"":""US""},
    {""Id"":54,""Name"":""* Other Id Type"",""ParentId"":5,""SystemName"":""OTHER_ID_TYPE"",""Description"":""This is the type of any other identifier, such as registration numbers issued by other authorities than listed above."",""Example"":""CEDULA""},
    {""Id"":55,""Name"":""* Other Id Number"",""ParentId"":5,""SystemName"":""OTHER_ID_NUMBER"",""Description"":""This is the actual ID number."",""Example"":""123121234""},
    {""Id"":56,""Name"":""Other Id Country"",""ParentId"":5,""SystemName"":""OTHER_ID_COUNTRY"",""Description"":""This is the country that issued the ID number."",""Example"":""MX""},
    {""Id"":57,""Name"":""Trusted Id Type"",""ParentId"":5,""SystemName"":""TRUSTED_ID_TYPE"",""Description"":""The type of ID that is to be trusted.  See the note below"",""Example"":""TRUE_SSN""},
    {""Id"":58,""Name"":""Trusted Id Number"",""ParentId"":5,""SystemName"":""TRUSTED_ID_NUMBER"",""Description"":""The trusted unique ID.  "",""Example"":""123-45-1234""},
    {""Id"":59,""Name"":""Account Number"",""ParentId"":6,""SystemName"":""ACCOUNT_NUMBER"",""Description"":""This is an account number such as a bank account, credit card number, etc."",""Example"":""1234-1234-1234-1234""},
    {""Id"":60,""Name"":""Account Domain"",""ParentId"":6,""SystemName"":""ACCOUNT_DOMAIN"",""Description"":""This is the domain the account number is valid in."",""Example"":""VISA""},
    {""Id"":61,""Name"":""Duns Number"",""ParentId"":6,""SystemName"":""DUNS_NUMBER"",""Description"":""The unique identifier for a company https://www.dnb.com/duns-number.html"",""Example"":""123123""},
    {""Id"":62,""Name"":""Npi Number"",""ParentId"":6,""SystemName"":""NPI_NUMBER"",""Description"":""A unique ID for covered health care providers.  https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/"",""Example"":""123123""},
    {""Id"":63,""Name"":""Lei Number"",""ParentId"":6,""SystemName"":""LEI_NUMBER"",""Description"":""A unique ID for entities involved in financial transactions.  https://en.wikipedia.org/wiki/Legal_Entity_Identifier"",""Example"":""123123""},
    {""Id"":64,""Name"":""Website Address"",""ParentId"":7,""SystemName"":""WEBSITE_ADDRESS"",""Description"":""This is a website address, usually only present for organization entities."",""Example"":""somecompany.com""},
    {""Id"":65,""Name"":""Email Address"",""ParentId"":7,""SystemName"":""EMAIL_ADDRESS"",""Description"":""This is the actual email address."",""Example"":""someone@somewhere.com""},
    {""Id"":66,""Name"":""Linkedin"",""ParentId"":7,""SystemName"":""LINKEDIN"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":67,""Name"":""Facebook"",""ParentId"":7,""SystemName"":""FACEBOOK "",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":68,""Name"":""Twitter"",""ParentId"":7,""SystemName"":""TWITTER"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":69,""Name"":""Skype"",""ParentId"":7,""SystemName"":""SKYPE"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":70,""Name"":""Zoomroom"",""ParentId"":7,""SystemName"":""ZOOMROOM"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":71,""Name"":""Instagram"",""ParentId"":7,""SystemName"":""INSTAGRAM"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":72,""Name"":""Whatsapp"",""ParentId"":7,""SystemName"":""WHATSAPP"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":73,""Name"":""Signal"",""ParentId"":7,""SystemName"":""SIGNAL"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":74,""Name"":""Telegram"",""ParentId"":7,""SystemName"":""TELEGRAM"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":75,""Name"":""Tango"",""ParentId"":7,""SystemName"":""TANGO"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":76,""Name"":""Viber"",""ParentId"":7,""SystemName"":""VIBER"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":77,""Name"":""Wechat"",""ParentId"":7,""SystemName"":""WECHAT"",""Description"":""This is the unique identifier in this domain."",""Example"":""xxxxx""},
    {""Id"":78,""Name"":""Employer Name"",""ParentId"":8,""SystemName"":""EMPLOYER_NAME"",""Description"":""This is the name of the organization the person is employed by. "",""Example"":""ABC Company""},
    {""Id"":79,""Name"":""Group Association Type"",""ParentId"":8,""SystemName"":""GROUP_ASSOCIATION_TYPE"",""Description"":""This is the type of group an entity belongs to."",""Example"":""MEMBER""},
    {""Id"":80,""Name"":""Group Association Org Name"",""ParentId"":8,""SystemName"":""GROUP_ASSOCIATION_ORG_NAME"",""Description"":""This is the name of the organization an entity belongs to. "",""Example"":""Group name""},
    {""Id"":81,""Name"":""Group Assn Id Type"",""ParentId"":8,""SystemName"":""GROUP_ASSN_ID_TYPE"",""Description"":""When the group a person is associated with has a registered identifier, place the type of identifier here."",""Example"":""DUNS""},
    {""Id"":82,""Name"":""Group Assn Id Number"",""ParentId"":8,""SystemName"":""GROUP_ASSN_ID_NUMBER"",""Description"":""When the group a person is associated with has a registered identifier, place the identifier here."",""Example"":""12345""},
    {""Id"":83,""Name"":""Phone Extension"",""ParentId"":3,""SystemName"":""PHONE_EXT"",""Description"":""This is the actual phone number extension."",""Example"":""101""},
    {""Id"":85,""Name"":""Latitude & Longitude"",""ParentId"":84,""SystemName"":""GEO_LATLONG"",""Description"":""Latitude and longitude in degrees in a single column and separated by a comma. If this is provided, then neither LATITUDE or LONGITUDE should be provided."",""Example"":""40.7269223,-73.9817648""},
    {""Id"":86,""Name"":""Latitude"",""ParentId"":84,""SystemName"":""GEO_LATITUDE"",""Description"":""Latitude in degrees. If this is specified then LONGITUDE must be provided."",""Example"":""40.7269223""},
    {""Id"":87,""Name"":""Longitude"",""ParentId"":84,""SystemName"":""GEO_LONGITUDE"",""Description"":""Longitude in degrees. If this is specified then LATITUDE must be provided."",""Example"":""-73.9817648""},
    {""Id"":88,""Name"":""Precision"",""ParentId"":84,""SystemName"":""GEO_PRECISION"",""Description"":""optional) GEO Precision in meters. If this is not provided then the GEO location is assumed to be exact."",""Example"":""5.0""}]";
    }
}