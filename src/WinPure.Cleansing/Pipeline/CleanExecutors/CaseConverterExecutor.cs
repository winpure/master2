using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;
using WinPure.Configuration.Models.Configuration;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class CaseConverterExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly CaseConverterSetting _setting;

    public CaseConverterExecutor(CaseConverterSetting setting)
    {
        _setting = setting;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        Parallel.ForEach(context.CleansingData.Where(x => !string.IsNullOrWhiteSpace(x.Value)), context.ParallelOptions, x =>
        {
            try
            {
                var val = x.Value;

                if (string.IsNullOrWhiteSpace(val))
                {
                    return;
                }

                if (_setting.ToLowerCase)
                {
                    val = val.ToLower();
                }

                if (_setting.ToUpperCase)
                {
                    val = val.ToUpper();
                }

                if (_setting.ToProperCase)
                {
                    val = SetProperCase(val, context.ProperCaseConfiguration);
                }

                x.Value = val;
            }
            catch (Exception ex)
            {
                context.Logger.Error($"Error on {GetType().Name}. Value={x.Value} Message={ex.Message}", ex);
                context.Exceptions.Add(new PipelineExceptionData
                {
                    Executor = GetType().Name,
                    OriginalException = ex,
                    OriginalValue = x.Value
                });
            }
        });

        await next(context);
    }

    private string SetProperCase(string input, ProperCaseConfiguration properCaseConfiguration)
    {
        //var cultureInfo = new CultureInfo("en-US", false).TextInfo;
        input = input.ToLower();
        //input = cultureInfo.ToTitleCase(input);
        //return input;

        char[] chars = input.ToCharArray();
        chars[0] = Char.ToUpper(chars[0]);
        var delimiterList = _setting.ProperCaseSettings.UseDelimiter
            ? properCaseConfiguration.Delimeters.ToCharArray()
            : new[] { ' ' };

        for (int i = 1; i + 1 < chars.Length; i++)
        {
            if (delimiterList.Contains(chars[i]))
            {
                chars[i + 1] = Char.ToUpper(chars[i + 1]);
            }
        }

        var words = new string(chars).Split(' ');

        if (_setting.ProperCaseSettings.UsePrefix)
        {
            foreach (var prefix in properCaseConfiguration.PrefixList.OrderBy(x => x.Length))
            {
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].ToLower().StartsWith(prefix.ToLower()) && !String.Equals(words[i], prefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        words[i] = prefix +
                                   words[i].Substring(prefix.Length, 1).ToUpper() +
                                   words[i].Substring(prefix.Length + 1);
                    }
                }
            }
        }

        if (_setting.ProperCaseSettings.UseExceptions)
        {
            for (int i = 0; i < words.Length; i++)
            {
                var exception = properCaseConfiguration.ExceptionList.FirstOrDefault(x => string.Equals(x, words[i], StringComparison.InvariantCultureIgnoreCase));
                if (exception != null)
                {
                    words[i] = exception;
                }
            }
        }

        return String.Join(" ", words);
    }
}