using System.Collections.Generic;

namespace WinPure.Cleansing.Models.ContextData;

internal class SplitResult
{
    public List<string> SplitByRegex { get; set; }
    public List<string> SplitIntoWords { get; set; }

    public string EmailAccount { get; set; }
    public string EmailDomain { get; set; }
    public string EmailCountry { get; set; }
    public string EmailSubDomain { get; set; }

    public string SplitEmail { get; set; }
    public string SplitEmailName { get; set; }

    public string PhoneCountry { get; set; }
    public string PhoneNumber { get; set; }

    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }
    public int? Hour { get; set; }
    public int? Minute { get; set; }
    public int? Second { get; set; }
}