namespace WinPure.Configuration.Models.Database
{
    public class CleansingAiConfigurationEntity
    {
        public string AiType { get; set; }
        public List<CleanAiMappedField> MappedFields { get; set; } = new();
        public CleanAiFieldOptions Options { get; set; } = new();

        public CleansingAiFieldType ToModel() =>
         new CleansingAiFieldType
            {
                AiType = AiType,
                MappedFields = MappedFields == null
                    ? new List<CleanAiMappedField>()
                    : MappedFields.Select(m => new CleanAiMappedField
                    {
                        Name = m.Name,
                        MapType = m.MapType,
                        Precision = m.Precision
                    }).ToList(),
                Options = Options == null
                    ? null
                    : new CleanAiFieldOptions
                    {
                        TrailingSpaces = Options.TrailingSpaces,
                        Comma = Options.Comma,
                        Dots = Options.Dots,
                        Hyphens = Options.Hyphens,
                        Apostrophes = Options.Apostrophes,
                        LeadingSpaces = Options.LeadingSpaces,
                        Letters = Options.Letters,
                        Numbers = Options.Numbers,
                        NonPrintable = Options.NonPrintable,
                        Spaces = Options.Spaces,
                        MultipleSpaces = Options.MultipleSpaces,
                        NewLine = Options.NewLine,
                        TabChar = Options.TabChar,
                        AddressParser = Options.AddressParser,
                        Pattern = Options.Pattern
                    }
            };
    }
}
