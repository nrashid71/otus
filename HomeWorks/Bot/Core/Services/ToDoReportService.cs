namespace Bot;

public class ToDoReportService: IToDoReportService
{
    IToDoService _toDoService;

    public ToDoReportService(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId, CancellationToken ct)
    {
        return (total: _toDoService.GetAllByUserId(userId, ct).Result.Count,
            completed: _toDoService.GetAllByUserId(userId, ct).Result.Count - _toDoService.GetActiveByUserId(userId, ct).Result.Count,
            active: _toDoService.GetActiveByUserId(userId, ct).Result.Count,
            generatedAt: DateTime.Now);
    }
}