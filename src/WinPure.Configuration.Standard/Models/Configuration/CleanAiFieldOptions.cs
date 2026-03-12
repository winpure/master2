namespace WinPure.Configuration.Models.Configuration;

public class CleanAiFieldOptions
{
    [DisplayNameAttribute("Trailing Spaces")]
    public bool TrailingSpaces { get; set; }

    [DisplayNameAttribute("Commas")]
    public bool Comma { get; set; }

    [DisplayNameAttribute("Dots")]
    public bool Dots { get; set; }

    [DisplayNameAttribute("Hyphens")]
    public bool Hyphens { get; set; }

    [DisplayNameAttribute("Apostrophes")]
    public bool Apostrophes { get; set; }

    [DisplayNameAttribute("Leading Spaces")]
    public bool LeadingSpaces { get; set; }

    [DisplayNameAttribute("Letters")]
    public bool Letters { get; set; }

    [DisplayNameAttribute("Numbers")]
    public bool Numbers { get; set; }

    [DisplayNameAttribute("Non Printable Characters")]
    public bool NonPrintable { get; set; }

    [DisplayNameAttribute("Spaces")]
    public bool Spaces { get; set; }

    [DisplayNameAttribute("Multiple Spaces")]
    public bool MultipleSpaces { get; set; }

    [DisplayNameAttribute("New Line")]
    public bool NewLine { get; set; }

    [DisplayNameAttribute("Tab Character")]
    public bool TabChar { get; set; }

    [DisplayNameAttribute("Address Parser")]
    public bool AddressParser { get; set; }

    [DisplayNameAttribute("Regex Pattern")]
    public string Pattern { get; set; } = string.Empty;
}