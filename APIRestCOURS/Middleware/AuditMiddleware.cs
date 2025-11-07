using System.Diagnostics;

namespace APIRestCOURS.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Générer un identifiant de corrélation
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;

        // Ajouter l'ID de corrélation dans les headers de réponse
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        // Récupérer la clé API si présente (simulation)
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "anonymous";

        try
        {
            // Exécuter le reste du pipeline
            await _next(context);

            stopwatch.Stop();

            // Log de succès
            LogAudit(
                correlationId,
                startTime,
                context.Request.Path,
                context.Request.Method,
                apiKey,
                context.Response.StatusCode,
                "Success",
                stopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log d'échec
            LogAudit(
                correlationId,
                startTime,
                context.Request.Path,
                context.Request.Method,
                apiKey,
                500,
                $"Failure: {ex.Message}",
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }

    private void LogAudit(
        string correlationId,
        DateTime timestamp,
        string path,
        string method,
        string clientId,
        int statusCode,
        string result,
        long durationMs)
    {
        _logger.LogInformation(
            "[AUDIT] CorrelationId={CorrelationId} | Timestamp={Timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC | " +
            "Path={Path} | Method={Method} | ClientId={ClientId} | StatusCode={StatusCode} | " +
            "Result={Result} | Duration={Duration}ms",
            correlationId,
            timestamp,
            path,
            method,
            clientId,
            statusCode,
            result,
            durationMs
        );
    }
}

// Extension method pour faciliter l'ajout du middleware
public static class AuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditMiddleware>();
    }
}
