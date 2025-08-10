using Telegram.Bot;

namespace Bot;

public class DeadlineBackgroundTask : BackgroundTask //(TimeSpan delay, string name) : BackgroundTask(delay, name)
{
    private readonly INotificationService _notificationService;

    private readonly IUserRepository _userRepository;

    private readonly IToDoRepository _toDoRepository;

    public DeadlineBackgroundTask(TimeSpan delay,
        INotificationService notificationService,
        IUserRepository userRepository,
        IToDoRepository toDoRepository) :
        base(delay, nameof(DeadlineBackgroundTask))
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _toDoRepository = toDoRepository;
    }

    protected override async Task Execute(CancellationToken ct)
    {
        foreach (var toDoUser in await _userRepository.GetUsers(ct))
        {
            foreach (var toDoItem in await _toDoRepository.GetActiveWithDeadline(toDoUser.UserId, 
                                                                            DateTime.UtcNow.AddDays(-1).Date,
                                                                                DateTime.UtcNow.Date,
                                                                                    ct))
            {
                await _notificationService.ScheduleNotification(
                                                    toDoUser.UserId,
                                                    $"Dealine_{toDoItem.Id}",
                                                    $"Ой! Вы пропустили дедлайн по задаче {toDoItem.Name}",
                                                    DateTime.UtcNow,
                                                    ct);
            }
        }
    }
}