using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class ColumnSplitterExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly List<ColumnSplitSetting> _settings;
    private static readonly List<string> PublicDomains = new List<string> { "com", "org", "net", "edu", "int", "gov", "mil", "arpa" };
    private readonly Regex _emailWithNameRegexPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.Compiled);
    private readonly Regex _emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly Regex _regexTelephoneParts = new Regex(@"[^\d]");

    public ColumnSplitterExecutor(List<ColumnSplitSetting> settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        Regex copyRegex = null;
        var settingWithRegex = _settings.FirstOrDefault(s => !string.IsNullOrEmpty(s.RegexCopy));
        if (settingWithRegex != null)
        {
            copyRegex = new Regex(settingWithRegex.RegexCopy, RegexOptions.Compiled);
        }

        var settingSplitIntoWords = _settings.FirstOrDefault(s => s.SplitIntoWords != null);
        var splitEmailAddressIntoAccountDomainAndZone = _settings.Any(s => s.SplitEmailAddressIntoAccountDomainAndZone);

        var splitNameAndEmailAddress = _settings.Any(s => s.SplitNameAndEmailAddress);
        var splitTelephoneIntoInternationalCodeAndPhoneNumber = _settings.Any(s => s.SplitTelephoneIntoInternationalCodeAndPhoneNumber);
        var splitDatetime = _settings.Any(s => s.SplitDatetime);

        //foreach (var x in context.CleansingData.Where(x => !string.IsNullOrWhiteSpace(x.Value)))
        Parallel.ForEach(context.CleansingData.Where(x => !string.IsNullOrWhiteSpace(x.Value)), context.ParallelOptions, x =>
        {
            var fieldValue = x.Value;

            if (context.ColumnType == typeof(string))
            {
                x.SplitResult = new SplitResult();
                #region SplitIntoWords
                try
                {
                    if (copyRegex != null)
                    {
                        //find items that matches with our pattern
                        var copyMatches = copyRegex.Matches(fieldValue);
                        x.SplitResult.SplitByRegex = new List<string>();
                        foreach (Match valMatch in copyMatches)
                        {
                            x.SplitResult.SplitByRegex.Add(valMatch.Value);
                            fieldValue = fieldValue.Replace(valMatch.Value, "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProcessException("split with regex", x.Value, ex, context);
                }

                try
                {
                    if (settingSplitIntoWords != null)
                    {
                        var splitResult = fieldValue.Split(new[] { settingSplitIntoWords.SplitIntoWords.SplitSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        x.SplitResult.SplitIntoWords = new List<string>();
                        x.SplitResult.SplitIntoWords.AddRange(splitResult.ToList());
                    }
                }
                catch (Exception ex)
                {
                    ProcessException("split into words", x.Value, ex, context);
                }
                #endregion

                #region SplitEmailAddressIntoAccountDomainAndZone
                try
                {
                    if (splitEmailAddressIntoAccountDomainAndZone)
                    {

                        var match = _emailWithNameRegexPattern.Match(fieldValue);

                        if (match.Success)
                        {
                            var spl = fieldValue.Split('@');
                            var emailName = spl[0];
                            var emailDomain = spl[1];
                            var partsOfEmailDomain = emailDomain.Split('.');

                            //var newVal = address.User;
                            var emailPartToSplit = emailName;

                            x.SplitResult.EmailAccount = emailPartToSplit;

                            emailPartToSplit = partsOfEmailDomain[partsOfEmailDomain.Length - 2] + "." +
                                               partsOfEmailDomain[partsOfEmailDomain.Length - 1];

                            x.SplitResult.EmailDomain = emailPartToSplit;

                            emailPartToSplit = partsOfEmailDomain[partsOfEmailDomain.Length - 1];
                            if (!PublicDomains.Contains(emailPartToSplit))
                            {
                                x.SplitResult.EmailCountry = emailPartToSplit;
                            }

                            var host = "." + partsOfEmailDomain[partsOfEmailDomain.Length - 2] + "." +
                                       partsOfEmailDomain[partsOfEmailDomain.Length - 1];

                            emailPartToSplit = emailDomain.Replace(host, "");

                            x.SplitResult.EmailSubDomain = emailPartToSplit;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProcessException("in SplitEmailAddressIntoAccountDomainAndZone", x.Value, ex, context);
                }
                #endregion

                #region SplitNameAndEmailAddress
                try
                {
                    if (splitNameAndEmailAddress)
                    {
                        var newName = string.Empty;
                        var newEmail = string.Empty;
                        //find items that matches with our pattern
                        var emailMatches = _emailRegex.Matches(fieldValue);

                        foreach (Match emailMatch in emailMatches)
                        {
                            newEmail = emailMatch.Value;
                            newName = fieldValue.Replace("<" + emailMatch.Value + ">", "")
                                .Replace(emailMatch.Value, "")
                                .Trim();
                            break;
                        }

                        x.SplitResult.SplitEmail = newEmail;
                        x.SplitResult.SplitEmailName = newName;
                    }
                }
                catch (Exception ex)
                {
                    ProcessException("in SplitNameAndEmailAddress", x.Value, ex, context);
                }
                #endregion

                #region SplitTelephoneIntoInternationalCodeAndPhoneNumber
                try
                {
                    if (splitTelephoneIntoInternationalCodeAndPhoneNumber)
                    {
                        var list = new List<string>();
                        list.AddRange(context.PhoneCodes.Keys);
                        list.AddRange(context.PhoneCodes.Values);

                        fieldValue = _regexTelephoneParts.Replace(fieldValue, "");

                        if (list.Any(l => fieldValue.StartsWith(l.ToLower())))
                        {
                            var first = list.First(l => fieldValue.StartsWith(l.ToLower()));
                            x.SplitResult.PhoneCountry = first;
                            x.SplitResult.PhoneNumber = fieldValue.Remove(0, first.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProcessException("in SplitTelephoneIntoInternationalCodeAndPhoneNumber", x.Value, ex, context);
                }
                #endregion
            }

            #region SplitDatetime
            if (splitDatetime && DateTime.TryParse(fieldValue, out var date))
            {
                x.SplitResult = new SplitResult();
                x.SplitResult.Year = date.Year;
                x.SplitResult.Month = date.Month;
                x.SplitResult.Day = date.Day;
                x.SplitResult.Hour = date.Hour;
                x.SplitResult.Minute = date.Minute;
                x.SplitResult.Second = date.Second;
            }
            #endregion
        });

        await next(context);
    }

    private void ProcessException(string splitOperation, string value, Exception exception, CleansingContext context)
    {
        context.Logger.Error($"Error on {GetType().Name} {splitOperation}. Value={value} Message={exception.Message}", exception);
        context.Exceptions.Add(new PipelineExceptionData
        {
            Executor = GetType().Name,
            OriginalException = exception,
            OriginalValue = value
        });
    }
}