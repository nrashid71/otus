namespace Bot;

public class ToDoReportService: IToDoReportService
{
    InMemoryToDoRepository _botTasks;

    public ToDoReportService(InMemoryToDoRepository inMemoryToDoRepository)
    {
        _botTasks = inMemoryToDoRepository;
    }

    public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
    {
        return (total: _botTasks.GetCount(userId),
            completed: _botTasks.GetCount(userId)-_botTasks.CountActive(userId),
            active: _botTasks.CountActive(userId),
            generatedAt: DateTime.Now);
    }
}