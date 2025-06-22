namespace Bot;

public interface IToDoService
{
    Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId);
    Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId);
    Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline, ToDoList? list);
    Task MarkCompleted(Guid id);
    Task Delete(Guid id);
    Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix);
    Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct);
    Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct);
}