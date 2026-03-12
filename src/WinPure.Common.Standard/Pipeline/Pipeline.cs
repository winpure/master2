using System.Collections.Generic;
using System.Threading.Tasks;
using WinPure.Common.Exceptions;

namespace WinPure.Common.Pipeline
{
    internal class Pipeline<TContext, TResult> : IPipeline<TContext, TResult>
        where TContext : IPipelineContext<TResult>
    {
        private readonly LinkedList<IPipelineMiddleware<TContext>> _middlewares;

        public Pipeline(LinkedList<IPipelineMiddleware<TContext>> middlewares)
        {
            if (middlewares == null || !middlewares.Any())
            {
                throw new WinPureArgumentException($"Middleware {nameof(middlewares)}: pipeline should contain steps");
            }

            _middlewares = middlewares;
        }

        public async Task<TResult> Execute(TContext context)
        {
            var currentStep = _middlewares.Last;
            Func<TContext, Task> previous = ctx => Task.CompletedTask;
            Func<TContext, Task> pipelineAction;
            do
            {
                var executeAction = new Func<TContext, Func<TContext, Task>, Task>(currentStep.Value.Execute);
                var next = new Func<TContext, Task>(previous);

                pipelineAction = (ctx) => executeAction(ctx, next);

                previous = pipelineAction;
                currentStep = currentStep.Previous;
            } while (currentStep != null);

            await pipelineAction(context);

            return await context.GetResult();
        }
    }
}
