namespace Bot;

public interface IToDoRepository
{
    /// <summary>
    /// Это публичный метод GetByGuid не заявлен в домашнем задании, но без него никак не реализуешь IToDoService.MarkCompleted
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ToDoItem?> GetByGuid(Guid id, CancellationToken ct);

    Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct);
    
    //Возвращает ToDoItem для UserId со статусом Active
    Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct);
    
    Task Add(ToDoItem item, CancellationToken ct);
    
    Task Update(ToDoItem item, CancellationToken ct);
    
    Task Delete(Guid id, CancellationToken ct);
    
    //Проверяет есть ли задача с таким именем у пользователя
    Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct);
    
    //Возвращает количество активных задач у пользователя
    Task<int> CountActive(Guid userId, CancellationToken ct);
    
    Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct); 
    Task<IReadOnlyList<ToDoItem>> GetActiveWithDeadline(Guid userId, DateTime from, DateTime to, CancellationToken ct);
}