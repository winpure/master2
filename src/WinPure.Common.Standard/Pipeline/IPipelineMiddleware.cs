using System.Threading.Tasks;

namespace WinPure.Common.Pipeline
{
    internal interface IPipelineMiddleware<TContext>
    {
        Task Execute(TContext context, Func<TContext, Task> next);
    }
}
