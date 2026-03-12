using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinPure.Cleansing.Enums;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class WordManagerExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly List<WordManagerSetting> _settings;
    private static readonly char[] SpecialChars = { '\\', '[', '*', '(', ')', '.', '?', '%', '^', '&', '@', '#', ']', '/', '!', '<', '>', '+', '-', '"', '\'', '{', '}', '‘', '”' };

    public WordManagerExecutor(List<WordManagerSetting> settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        if (context.ColumnType == typeof(string))
        {
            Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
            {
                try
                {
                    foreach (var operation in _settings)
                    {
                        var value = x.Value;
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }

                        var replace = operation.ToDelete ? "" : operation.ReplaceValue;

                        if (operation.ReplaceType == WordManagerReplaceType.WholeWord)
                        {
                            if (SpecialChars.Select(s => s.ToString()).Contains(operation.SearchValue))
                            {
                                if (value.Trim().StartsWith(operation.SearchValue + " "))
                                {
                                    value = replace + " " + value.Trim().Substring(2, value.Trim().Length - 2);
                                }
                                else if (value.Trim().EndsWith(" " + operation.SearchValue))
                                {
                                    value = value.Trim().Substring(0, value.Trim().Length - 2) + " " + replace;
                                }
                                else if (value.Trim().Contains(" " + operation.SearchValue + " "))
                                {
                                    value = value.Replace(" " + operation.SearchValue + " ", " " + replace + " ");
                                }
                                else if (value.Trim() == operation.SearchValue)
                                {
                                    value = replace;
                                }
                            }
                            else
                            {
                                if (operation.SearchValue.IndexOfAny(SpecialChars) != -1)
                                {
                                    var pattern = operation.SearchValue;
                                    for (int i = 0; i < SpecialChars.Length; i++)
                                    {
                                        pattern = pattern.Replace(SpecialChars[i].ToString(), "\\" + SpecialChars[i]);
                                    }
                                    pattern = @"\b" + pattern + @"\b";
                                    value = Regex.Replace(value, pattern, replace);
                                }
                                else
                                {
                                    var pattern = @"\b" + operation.SearchValue + @"\b";
                                    value = Regex.Replace(value, pattern, replace);
                                }
                            }

                        }
                        else if (operation.ReplaceType == WordManagerReplaceType.WholeValue)
                        {
                            if (value == operation.SearchValue)
                            {
                                value = value.Replace(operation.SearchValue, replace);
                            }
                        }
                        else if (operation.ReplaceType == WordManagerReplaceType.AnyPart)
                        {
                            value = value.Replace(operation.SearchValue, replace);
                        }
                        x.Value = value;
                    }
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
        }

        await next(context);
    }
}