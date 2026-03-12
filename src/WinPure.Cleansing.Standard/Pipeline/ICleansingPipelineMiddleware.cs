using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline;

internal interface ICleansingPipelineMiddleware<TContext> : IPipelineMiddleware<TContext>
{
}