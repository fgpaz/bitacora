using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuestrasCuentitas.Bitacora.Application.Commands.Telegram;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Infrastructure.Options;

namespace NuestrasCuentitas.Bitacora.Api.Workers;

/// <summary>
/// Minimal hosted service that dispatches due Telegram reminders via SendReminderCommand.
/// Polls every 60 seconds for enabled ReminderConfigs with NextFireAtUtc less-than-now.
/// Backpressure: a semaphore bounds concurrent dispatches to a configurable limit (default: 3).
/// </summary>
public sealed class ReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReminderWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(60);
    private readonly SemaphoreSlim _dispatchSemaphore;
    private readonly int _maxConcurrentDispatches;

    public ReminderWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ReminderWorker> logger,
        IOptions<ReminderConfig> config)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _maxConcurrentDispatches = Math.Max(1, config.Value.MaxRetryAttempts);
        _dispatchSemaphore = new SemaphoreSlim(_maxConcurrentDispatches, _maxConcurrentDispatches);
        _logger.LogInformation("ReminderWorker: max concurrent dispatches set to {Max}", _maxConcurrentDispatches);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderWorker started. Polling interval: {Interval}s, max concurrent dispatches: {Max}",
            _pollInterval.TotalSeconds, _maxConcurrentDispatches);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollAndDispatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "ReminderWorker: unhandled error during poll cycle");
            }

            try
            {
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("ReminderWorker stopped");
    }

    private async Task PollAndDispatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var reminderRepo = scope.ServiceProvider.GetRequiredService<IReminderConfigRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var nowUtc = DateTime.UtcNow;
        var dueReminders = await reminderRepo.GetDueRemindersAsync(nowUtc, cancellationToken);

        if (dueReminders.Count == 0)
        {
            return;
        }

        _logger.LogInformation("ReminderWorker: found {Count} due reminders", dueReminders.Count);

        // Backpressure: cap concurrent dispatches to avoid unbounded fan-out.
        // All dispatches are fire-and-forget w.r.t. the poll cycle but bounded per-cycle.
        var tasks = new List<Task>(dueReminders.Count);
        foreach (var reminder in dueReminders)
        {
            // Block if max concurrent dispatches is reached — wait for a slot.
            await _dispatchSemaphore.WaitAsync(cancellationToken);

            var traceId = Guid.NewGuid();
            tasks.Add(DispatchWithReleaseAsync(mediator, reminder.ReminderConfigId, traceId, cancellationToken));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ReminderWorker: some dispatches failed in batch");
        }
    }

    private async Task DispatchWithReleaseAsync(IMediator mediator, Guid reminderConfigId, Guid traceId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(
                new SendReminderCommand(reminderConfigId, traceId),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "ReminderWorker: failed to dispatch SendReminderCommand for config {ReminderConfigId}, trace {TraceId}",
                reminderConfigId, traceId);
        }
        finally
        {
            _dispatchSemaphore.Release();
        }
    }
}
