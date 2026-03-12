using WinPure.Configuration.Enums;

namespace WinPure.Configuration.Helper;

internal static partial class DatabaseInitiator
{
    static List<CleansingAiConfigurationEntity> GetCleansingAiTypes() =>
    [
        new CleansingAiConfigurationEntity
        {
            AiType = "Persons Name",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "name", MapType = CleanAiMapType.Exact, Precision = 100m },
                new CleanAiMappedField { Name = "contact", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "firstname", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "surname", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "fullname", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "middlename", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "owner", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "first name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "last name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "middle name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "client name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "member name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "employee name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "nickname", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "alias", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "maiden name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "legal name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "user", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "candidate", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "applicant", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "spouse", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "trustee", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "donor", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "nombre", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = true,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Address",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "addr", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "address", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "street", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "full address", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "mailing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "billing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "shipping", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "delivery", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "premise", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "property", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = false,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "City",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "city", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "locality", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "region", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "town", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "mailing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "billing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "shipping", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "delivery", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "local ", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "State",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "state", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "province", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "region", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "mailing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "billing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "shipping", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "delivery", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = true,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Postal Code",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "postal code", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "zip", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "zipcode", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "mailing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "billing", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "shipping", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "delivery", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "PC", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = true,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Country",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "country", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "nation", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Company",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "company", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "organization", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "org", MapType = CleanAiMapType.Exact, Precision = 100m },
                new CleanAiMappedField { Name = "business", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "vendor", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "supplier", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "account name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "firm", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "DBA", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "clientcompany", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "employer", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "manufacturer", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "distributor", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "brand name", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "agency", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "parent company", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "corporation", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "entity", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "subsidiary", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "customer", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = true,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = true,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Phone",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "telephone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "tel", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "work phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "business phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "office phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "home phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "main phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "primary phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "alt phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "secondary phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "direct phone", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "number", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "landline", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Phone Ext",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "Ext", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "extension", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = false,
                Comma = false,
                Dots = false,
                Hyphens = false,
                Apostrophes = false,
                LeadingSpaces = false,
                Letters = false,
                Numbers = false,
                NonPrintable = false,
                Spaces = false,
                MultipleSpaces = false,
                NewLine = false,
                TabChar = false,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Mobile",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "mobile", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "cell", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "cellphone", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Email",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "email", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "alt email", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "Fax",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "fax", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = true,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "",
            }
        },

        new CleansingAiConfigurationEntity
        {
            AiType = "website",
            MappedFields = new List<CleanAiMappedField>
            {
                new CleanAiMappedField { Name = "homepage", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "domain", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "url", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "site", MapType = CleanAiMapType.Contains, Precision = 0m },
                new CleanAiMappedField { Name = "webpage", MapType = CleanAiMapType.Contains, Precision = 0m },
            },
            Options = new CleanAiFieldOptions
            {
                TrailingSpaces = true,
                Comma = true,
                Dots = false,
                Hyphens = true,
                Apostrophes = true,
                LeadingSpaces = true,
                Letters = false,
                Numbers = false,
                NonPrintable = true,
                Spaces = true,
                MultipleSpaces = true,
                NewLine = true,
                TabChar = true,
                AddressParser = false,
                Pattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$",
            }
        }

    ];
}