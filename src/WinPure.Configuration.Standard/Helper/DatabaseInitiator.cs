namespace WinPure.Configuration.Helper;

internal static partial class DatabaseInitiator
{
    public static void InitiateDatabase(IServiceProvider serviceProvider)
    {
        var ctx = serviceProvider.GetService(typeof(WinPureConfigurationContext)) as WinPureConfigurationContext;
        InitiateDatabase(ctx);
    }

    public static void InitiateDatabase(WinPureConfigurationContext context)
    {
        //context.Database.EnsureCreated();
        context.Database.Migrate();
        Seed(context);
    }

    private static void Seed(WinPureConfigurationContext context)
    {
        if (!context.AutomationStepsDictionary.Any())
        {
            context.AutomationStepsDictionary.AddRange(GetAutomationSteps);
        }
        else if (context.AutomationStepsDictionary.Count() < 7)
        {
            context.AutomationStepsDictionary.AddRange(GetAutomationMatchAiSteps);
        }

        if (!context.DictionaryName.Any())
        {
            context.DictionaryName.AddRange(GetDictionaryNames);
        }

        if (!context.ProperCaseSettings.Any(x => x.Name == ProperCaseNameHelper.Delimiters))
        {
            context.ProperCaseSettings.Add(GetProperCaseDelimitersSettings);
        }

        if (!context.ProperCaseSettings.Any(x => x.Name == ProperCaseNameHelper.Exceptions))
        {
            context.ProperCaseSettings.Add(GetProperCaseExceptionsSettings);
        }

        if (!context.ProperCaseSettings.Any(x => x.Name == ProperCaseNameHelper.Prefix))
        {
            context.ProperCaseSettings.Add(GetProperCasePrefixSettings);
        }

        if (!context.StatisticPatterns.Any())
        {
            context.StatisticPatterns.AddRange(GetStatisticPatterns);
        }

        if (!context.EntityResolutionMapping.Any())
        {
            context.EntityResolutionMapping.AddRange(GetEntityResolutionMappings);
        }

        if (!context.CleansingAiConfigurations.Any())
        {
            context.CleansingAiConfigurations.AddRange(GetCleansingAiTypes());
        }

        context.SaveChanges();
    }
}