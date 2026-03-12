namespace WinPure.Common.Pipeline
{
    internal interface IPipelineBuilder<TContext, TResult> where TContext : IPipelineContext<TResult>
    {
        IPipelineBuilder<TContext, TResult> Use(IPipelineMiddleware<TContext> middleware);

        IPipeline<TContext, TResult> Build();

        bool ContainsBuilder(Type type);
    }
}
