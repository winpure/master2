global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using WinPure.Common.Logger;
global using WinPure.Common.Pipeline;
global using WinPure.Configuration.Service;
global using WinPure.Matching.Enums;
global using WinPure.Matching.Helpers;
global using WinPure.Matching.Models.InternalModel;
global using WinPure.Matching.Pipeline;
global using WinPure.Matching.Services;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;


[assembly: InternalsVisibleTo("WinPure.Project")]
[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.API.Core")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]
namespace WinPure.Matching.DependencyInjection;

internal static partial class WinPureMatchingDependencyExtension
{
    public static void RegisterMatching(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRepresentationService>(_ => new RepresentationService());
        serviceCollection.AddTransient<IDataNormalizationService>(sp =>
            new DataNormalizationService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IDictionaryService>(),
                sp.GetRequiredService<IRepresentationService>()
            )
        );
        serviceCollection.AddTransient<IMatchService>(sp =>
            new MatchService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IDictionaryService>()
            )
        );
        serviceCollection.AddTransient<ISearchService>(sp =>
            new SearchService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IDictionaryService>()
            )
        );
    }
}