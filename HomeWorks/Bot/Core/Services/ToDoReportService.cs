namespace Bot;

public class ToDoReportService: IToDoReportService
{
    IToDoService _toDoService;

    public ToDoReportService(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
    {
        return (total: _toDoService.GetAllByUserId(userId).Result.Count,
            completed: _toDoService.GetAllByUserId(userId).Result.Count - _toDoService.GetActiveByUserId(userId).Result.Count,
            active: _toDoService.GetActiveByUserId(userId).Result.Count,
            generatedAt: DateTime.Now);
    }
}