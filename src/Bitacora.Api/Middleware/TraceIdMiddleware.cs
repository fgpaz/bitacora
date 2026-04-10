using NuestrasCuentitas.Bitacora.Api.Extensions;

namespace NuestrasCuentitas.Bitacora.Api.Middleware;

public sealed class TraceIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Guid.TryParse(context.Request.Headers["X-Trace-Id"], out var parsedTraceId) && parsedTraceId != Guid.Empty
            ? parsedTraceId
            : Guid.NewGuid();

        context.SetTraceId(traceId);
        context.Response.Headers["X-Trace-Id"] = traceId.ToString("D");

        await next(context);
    }
}
