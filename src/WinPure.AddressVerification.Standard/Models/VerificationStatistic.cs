using System.Collections.Generic;

namespace WinPure.AddressVerification.Models;

internal class VerificationStatistic
{
    public VerificationStatistic()
    {
        VerificationStatus = new Dictionary<string, int>
        {
            {"V",0 },
            {"P",0 },
            {"U",0},
            {"A",0 },
            {"C",0 },
            {"R",0 }
        };

        PostProcessing = new Dictionary<string, int>
        {
            {"5", 0},
            {"4", 0},
            {"3", 0},
            {"2",0},
            {"1", 0},
            {"0", 0}
        };

        PreProcessing = new Dictionary<string, int>
        {
            {"5",0},
            {"4",0},
            {"3",0},
            {"2",0},
            {"1",0},
            {"0",0}
        };

        ParsingStatus = new Dictionary<string, int>
        {
            {"I",0},
            {"U",0},
        };

        LexiconIdent = new Dictionary<string, int>
        {
            {"5",0},
            {"4",0},
            {"3",0},
            {"2",0},
            {"1",0},
            {"0",0}
        };

        ContextIdent = new Dictionary<string, int>
        {
            {"5", 0},
            {"4", 0},
            {"3",0},
            {"2",0},
            {"1", 0},
            {"0", 0}
        };

        Postcode = new Dictionary<string, int>
        {
            {"P8", 0},
            {"P7", 0},
            {"P6", 0},
            {"P5", 0},
            {"P4", 0},
            {"P3", 0},
            {"P2", 0},
            {"P1", 0},
            {"P0", 0}
        };
    }

    public int AddressSuccess { get; set; }
    public int GeoCodeSuccess { get; set; }
    public Dictionary<string, int> VerificationStatus { get; }
    public Dictionary<string, int> PostProcessing { get; }
    public Dictionary<string, int> PreProcessing { get; }
    public Dictionary<string, int> ParsingStatus { get; }
    public Dictionary<string, int> LexiconIdent { get; }
    public Dictionary<string, int> ContextIdent { get; }
    public Dictionary<string, int> Postcode { get; }
}