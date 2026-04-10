namespace NuestrasCuentitas.Bitacora.Api.Extensions;

public static class HttpContextTraceExtensions
{
    private const string TraceIdKey = "Bitacora.TraceId";

    public static Guid GetTraceId(this HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(TraceIdKey, out var value) &&
            value is Guid traceId &&
            traceId != Guid.Empty)
        {
            return traceId;
        }

        traceId = Guid.NewGuid();
        httpContext.Items[TraceIdKey] = traceId;
        return traceId;
    }

    public static void SetTraceId(this HttpContext httpContext, Guid traceId)
    {
        httpContext.Items[TraceIdKey] = traceId;
    }
}
