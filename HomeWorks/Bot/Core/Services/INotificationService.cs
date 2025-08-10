namespace Bot;

public interface INotificationService
{
    //Создает нотификацию. Если запись с userId и type уже есть, то вернуть false и не добавлять запись, иначе вернуть true
    Task<bool> ScheduleNotification(
        Guid userId,
        string type,
        string text,
        DateTime scheduledAt,
        CancellationToken ct);

    //Возвращает нотификации, у которых IsNotified = false && ScheduledAt <= scheduledBefore
    Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct);

    Task MarkNotified(Guid notificationId, CancellationToken ct);
}