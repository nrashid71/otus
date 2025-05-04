namespace Bot;

public interface IToDoService
{
    Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId);
    Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId);
    Task<ToDoItem> Add(ToDoUser user, string name);
    Task MarkCompleted(Guid id);
    Task Delete(Guid id);
    Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix); 
}