using System.Threading.Tasks;

namespace WinPure.Common.Pipeline
{
    internal interface IPipeline<TContext, TResult>
    {
        Task<TResult> Execute(TContext context);
    }
}
