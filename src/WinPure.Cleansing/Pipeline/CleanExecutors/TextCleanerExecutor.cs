using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class TextCleanerExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly List<TextCleanerSetting> _settings;

    public TextCleanerExecutor(List<TextCleanerSetting> settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
        {
            try
            {
                var textValue = x.Value;

                if (_settings.Any(s => !string.IsNullOrEmpty(s.ConvertEmptyToDefaultValue)) && string.IsNullOrEmpty(textValue))
                {
                    textValue = _settings.First(s => !string.IsNullOrEmpty(s.ConvertEmptyToDefaultValue)).ConvertEmptyToDefaultValue;
                }

                if (!string.IsNullOrEmpty(textValue))
                {
                    if (_settings.Any(s => s.ConvertOsToNaughts))
                    {
                        textValue = StringCleanerAndConvertor.ConvertOsToNaughts(textValue);
                    }

                    if (_settings.Any(s => s.ConvertOnesToLs))
                    {
                        textValue = StringCleanerAndConvertor.ConvertOnesToLs(textValue);
                    }

                    if (_settings.Any(s => s.ConvertNaughtsToOs))
                    {
                        textValue = StringCleanerAndConvertor.ConvertNaughtsToOs(textValue);
                    }

                    if (_settings.Any(s => s.ConvertLsToOnes))
                    {
                        textValue = StringCleanerAndConvertor.ConvertLsToOnes(textValue);
                    }

                    if (_settings.Any(s => !string.IsNullOrEmpty(s.RemoveCharacters)))
                    {
                        var charactersToRemove = _settings.First(s => !string.IsNullOrEmpty(s.RemoveCharacters)).RemoveCharacters;
                        textValue = String.Join("", textValue.Split(charactersToRemove.ToCharArray()));
                    }

                    if (_settings.Any(s => s.RemoveAllDigits))
                    {
                        textValue = StringCleanerAndConvertor.RemoveAllDigits(textValue);
                    }

                    if (_settings.Any(s => s.RemoveAllLetters))
                    {
                        textValue = StringCleanerAndConvertor.RemoveAllLetters(textValue);
                    }

                    if (_settings.Any(s => s.RemoveApostrophes))
                    {
                        textValue = StringCleanerAndConvertor.RemoveApostrophes(textValue);
                    }

                    if (_settings.Any(s => s.RemoveCommas))
                    {
                        textValue = StringCleanerAndConvertor.RemoveCommas(textValue);
                    }

                    if (_settings.Any(s => s.RemoveDots))
                    {
                        textValue = StringCleanerAndConvertor.RemoveDots(textValue);
                    }

                    if (_settings.Any(s => s.RemoveHyphens))
                    {
                        textValue = StringCleanerAndConvertor.RemoveHyphens(textValue);
                    }

                    if (_settings.Any(s => s.RemoveNonPrintableCharacters))
                    {
                        textValue = StringCleanerAndConvertor.RemoveNonPrintableCharacters(textValue, "");
                    }

                    if (_settings.Any(s => s.RemoveTabs))
                    {
                        textValue = StringCleanerAndConvertor.RemoveTabs(textValue);
                    }

                    if (_settings.Any(s => s.RemoveNewLine))
                    {
                        textValue = StringCleanerAndConvertor.RemoveNewLine(textValue);
                    }

                    if (_settings.Any(s => s.RemovePunctuation))
                    {
                        textValue = StringCleanerAndConvertor.RemovePunctuation(textValue);
                    }

                    if (_settings.Any(s => s.RegexExpression.Count > 0))
                    {
                        try
                        {
                            foreach (var regexExpression in _settings.Where(s => s.RegexExpression.Count > 0).SelectMany(s => s.RegexExpression).OrderBy(x => x.Id))
                            {
                                textValue = Regex.Replace(textValue, regexExpression.Expression,  regexExpression.Replacement);
                            }
                        }
                        catch
                        {
                            context.Logger.Information("Wrong Regex expression on cleaner settings");
                        }
                    }

                    if (_settings.Any(s => s.RemoveAllSpaces))
                    {
                        textValue = StringCleanerAndConvertor.RemoveAllSpaces(textValue);
                    }

                    if (_settings.Any(s => s.RemoveTrailingSpace))
                    {
                        textValue = StringCleanerAndConvertor.RemoveTrailingSpace(textValue);
                    }

                    if (_settings.Any(s => s.RemoveLeadingSpace))
                    {
                        textValue = StringCleanerAndConvertor.RemoveLeadingSpace(textValue);
                    }

                    if (_settings.Any(s => s.RemoveMultipleSpaces))
                    {
                        textValue = StringCleanerAndConvertor.RemoveMultipleSpace(textValue);
                    }
                }

                x.Value = textValue;
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
}