namespace Bot;

public interface IToDoService
{
    Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct);
    Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline, ToDoList? list, CancellationToken ct);
    Task MarkCompleted(Guid id, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct);
    Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct);
    Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct);
}