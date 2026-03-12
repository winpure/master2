using System.Threading.Tasks;

namespace WinPure.Common.Pipeline;

internal interface IPipelineContext<TResult>
{
    Task<TResult> GetResult();
}