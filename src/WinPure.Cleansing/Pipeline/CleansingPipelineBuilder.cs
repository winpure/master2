using System.Collections.Concurrent;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline;

internal class CleansingPipelineBuilder : PipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>>
{
}