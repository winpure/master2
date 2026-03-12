using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class GenderSplitExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly AddressAndGenderSplitSettings _settings;
    private NetGender.NetGender _netGender;

    public GenderSplitExecutor(AddressAndGenderSplitSettings settings)
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
            if (_settings.GenderColumns.Any())
            {
                _netGender = new NetGender.NetGender
                {
                    Static_Key_Name = "WinPure Ltd - winpure.com[1-developer]",
                    Static_Key = "9664-89A5-3D0F-F164",
                    Name_Style = "FML&FM",
                    Variable_Default = "FML",
                    Variable_Default2 = "FML",
                    Capitalization = "Mixed",
                    Company_Check = true,
                    Name_Variant_Check = true
                };
            }

            foreach (var x in context.CleansingData)
                //Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(x.GenderSplitValue))
                    {
                        _netGender.Name_In = x.GenderSplitValue;

                        _netGender.Parse();
                        if (string.IsNullOrEmpty(_netGender.Return_Code))
                        {
                            x.SplitGenderResult = new SplitGenderResult
                            {
                                Prefix = _netGender.Prefix,
                                First = _netGender.First,
                                Middle = _netGender.Middle,
                                Last = _netGender.Last,
                                Suffix = _netGender.Suffix,
                                Quality = _netGender.Name_Quality,
                                Gender = _netGender.Gender
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.Error($"Error on {GetType().Name} in gender split. Value={x.Value} Message={ex.Message}", ex);
                }
            }
            //);
        }
        catch (Exception ex)
        {
            context.Logger.Error($"Error on {GetType().Name} in gender split. Message={ex.Message}", ex);
            context.Exceptions.Add(new PipelineExceptionData
            {
                Executor = GetType().Name,
                OriginalException = ex
            });
        }
        await next(context);
    }
}