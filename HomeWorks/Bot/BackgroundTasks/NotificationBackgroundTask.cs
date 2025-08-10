using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class NotificationBackgroundTask : BackgroundTask
{
    private readonly INotificationService _notificationService;
    
    private readonly ITelegramBotClient _botClient;
    
    public  NotificationBackgroundTask(TimeSpan notificationTimePeriod, INotificationService notificationService, ITelegramBotClient bot) : base(notificationTimePeriod, nameof(NotificationBackgroundTask))
    {
        _notificationService = notificationService;
        _botClient = bot;
    }

    protected override async Task Execute(CancellationToken ct)
    {
        // await _botClient.SendMessage(1168793986,
        //     "Test",
        //     cancellationToken:ct);
        var notifications = await _notificationService.GetScheduledNotification(DateTime.UtcNow, ct);
        foreach (var notification in notifications)
        {
            await _botClient.SendMessage(notification.ToDoUser.TelegramUserId,
                notification.Text,
                cancellationToken:ct);
            await _notificationService.MarkNotified(notification.Id, ct);
        }
    }
}