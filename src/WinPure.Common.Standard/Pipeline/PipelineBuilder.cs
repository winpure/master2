using System.Collections.Generic;

namespace WinPure.Common.Pipeline
{
    internal abstract class PipelineBuilder<TContext, TResult> : IPipelineBuilder<TContext, TResult> where TContext : IPipelineContext<TResult>
    {
        private readonly LinkedList<IPipelineMiddleware<TContext>> _middlewares;

        protected PipelineBuilder()
        {
            _middlewares = new LinkedList<IPipelineMiddleware<TContext>>();
        }

        public IPipelineBuilder<TContext, TResult> Use(IPipelineMiddleware<TContext> middleware)
        {
            _middlewares.AddLast(middleware);

            return this;
        }

        public IPipeline<TContext, TResult> Build()
        {
            return new Pipeline<TContext, TResult>(_middlewares);
        }

        public bool ContainsBuilder(Type type)
        {
            return _middlewares.Any(x => x.GetType() == type);
        }
    }
}
