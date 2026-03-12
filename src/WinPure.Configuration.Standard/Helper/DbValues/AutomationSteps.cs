namespace WinPure.Configuration.Helper;

internal static partial class DatabaseInitiator
{
    static List<AutomationStepDictionaryEntity> GetAutomationSteps =>
    [
        new AutomationStepDictionaryEntity { Name = "Open project" },
        new AutomationStepDictionaryEntity { Name = "Apply cleaning" },
        new AutomationStepDictionaryEntity { Name = "Export cleaned data" },
        new AutomationStepDictionaryEntity { Name = "Run matching" },
        new AutomationStepDictionaryEntity { Name = "Export match result" },
        new AutomationStepDictionaryEntity { Name = "Run MatchAI" },
        new AutomationStepDictionaryEntity { Name = "Export MatchAI result" }
    ];

    static List<AutomationStepDictionaryEntity> GetAutomationMatchAiSteps =>
    [
        new AutomationStepDictionaryEntity { Name = "Run MatchAI" },
        new AutomationStepDictionaryEntity { Name = "Export MatchAI result" }
    ];
}