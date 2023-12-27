using Application.DataAccess;
using HotChocolate;
using HotChocolate.Execution;
using Throw;

namespace Application.GraphQL;

public class CachingMiddleware
{
    private readonly RequestDelegate _next;

    public CachingMiddleware(
        RequestDelegate next)
    {
        _next = next.ThrowIfNull();
    }

    public async ValueTask InvokeAsync(IRequestContext context, [Service] ICacheService cache)
    {
        // Before query execution
        var key = context.Document.ToString();
        var res = await cache.GetDataAsync<Dictionary<string, object?>?>(key);
        
        var isCached = res is not null;
        
        if (isCached)
        {
            context.Result = new QueryResult(res);
            return;
        }
        
        await _next(context).ConfigureAwait(false);

        // After query execution
        if (!isCached)
        {
            var toCache = ((QueryResult)context.Result).Data;
            await cache.SetDataAsync(key, toCache);
        }
    }
}