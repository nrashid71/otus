namespace Bot;

public class TodayBackgroundTask : BackgroundTask
{
    private readonly INotificationService _notificationService;
    
    private readonly IUserRepository _userRepository;
    
    private readonly IToDoRepository _toDoRepository;
    
    public TodayBackgroundTask(TimeSpan delay, INotificationService notificationService,
        IUserRepository userRepository,
        IToDoRepository toDoRepository): base(delay, nameof(TodayBackgroundTask))
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _toDoRepository = toDoRepository;
    }
    protected override async Task Execute(CancellationToken ct)
    {
        foreach (var toDoUser in await _userRepository.GetUsers(ct))
        {
             var toDoItems = await _toDoRepository.GetActiveWithDeadline(toDoUser.UserId,
                         DateTime.UtcNow.Date,
                         DateTime.UtcNow.AddDays(1).Date,
                         ct);
             if (toDoItems.Any())
             {
                 await _notificationService.ScheduleNotification(
                     toDoUser.UserId,
                     $"Today_{DateOnly.FromDateTime(DateTime.UtcNow)}",
                     "Список задач на сегодня: " + string.Join(", ", toDoItems.Select(i => i.Name).ToArray()),
                     DateTime.UtcNow,
                     ct);
             }
        }
    }
}