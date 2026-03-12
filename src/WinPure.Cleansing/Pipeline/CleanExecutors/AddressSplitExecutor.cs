using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LibPostalNet;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;
using WinPure.Configuration.DependencyInjection;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class AddressSplitExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly AddressAndGenderSplitSettings _settings;

    public AddressSplitExecutor(AddressAndGenderSplitSettings settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        try
        {
            var configuration = WinPureConfigurationDependency.Resolve<IConfigurationService>().Configuration;
            if (!configuration.LibpostalInitialized)
            {
                var path = Path.Combine(configuration.ErSettings.DataFolder, "api", "data", "libpostal");
                var retCode = libpostal.LibpostalSetupDatadir(path);
                retCode = libpostal.LibpostalSetupParserDatadir(path);
                retCode = libpostal.LibpostalSetupLanguageClassifierDatadir(path);
                configuration.LibpostalInitialized = true;
            }
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            foreach (var x in context.CleansingData)
                //Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
            {
                try
                {
                    var address = $"{x.AddressSplitValue} {x.CitySplitValue} {x.RegionSplitValue} {x.PostcodeSplitValue}".Trim();
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        var options = new LibpostalAddressParserOptions
                        {
                            Country = x.CountrySplitValue,
                            Language = null
                        };
                        var splitResult = libpostal.LibpostalParseAddress(address, options);

                        if (splitResult == null || !splitResult.Results.Any()) continue;

                        x.SplitAddressResult = new SplitAddressResult();
                        ProcessAddress(splitResult.Results, x.SplitAddressResult, cultureInfo);
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.Error(
                        $"Error on {GetType().Name} in gender split. Value={x.Value} Message={ex.Message}", ex);
                }
            }
            //);
        }
        catch (Exception ex)
        {
            context.Logger.Error($"Error on {GetType().Name} in address split. Message={ex.Message}", ex);
            context.Exceptions.Add(new PipelineExceptionData
            {
                Executor = GetType().Name,
                OriginalException = ex
            });
        }

        await next(context);
    }

    private void ProcessAddress(List<KeyValuePair<string, string>> results, SplitAddressResult model, CultureInfo cultureInfo)
    {
        foreach (var result in results)
        {
            var property = model.GetType().GetProperty(result.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                if (result.Key.ToLower() == "state")
                {
                    property.SetValue(model, result.Value.ToUpper());
                }
                else
                {
                    property.SetValue(model, cultureInfo.TextInfo.ToTitleCase(result.Value));
                }
            }
        }
    }
}