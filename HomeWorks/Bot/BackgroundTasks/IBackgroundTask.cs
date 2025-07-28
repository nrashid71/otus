namespace Bot;

public interface IBackgroundTask
{
    Task Start(CancellationToken ct);
}