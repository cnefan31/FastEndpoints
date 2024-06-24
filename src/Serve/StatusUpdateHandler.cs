using System.Runtime.CompilerServices;

namespace Serve;

public sealed class StatusUpdateHandler : IServerStreamCommandHandler<StatusStreamCommand, StatusUpdate>
{
    private readonly ILogger<StatusUpdateHandler> logger;

    public StatusUpdateHandler(ILogger<StatusUpdateHandler> logger)
    {
        this.logger = logger;
    }
    public async IAsyncEnumerable<StatusUpdate> ExecuteAsync(StatusStreamCommand command, [EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            for (var i = 1; !ct.IsCancellationRequested; i++)
            {
                try
                {
                    logger.LogInformation("Id =  {0}", i);
                    await Task.Delay(500, ct);
                }
                catch (TaskCanceledException)
                {
                    //do nothing
                }
                yield return new() { Message = $"Id: {command.Id} - {i}" };
            }
        }
    }
}
