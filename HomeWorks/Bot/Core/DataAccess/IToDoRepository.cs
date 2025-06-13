namespace Bot;

public interface IToDoRepository
{
    /// <summary>
    /// Это публичный метод GetByGuid не заявлен в домашнем задании, но без него никак не реализуешь IToDoService.MarkCompleted
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ToDoItem?> GetByGuid(Guid id);

    Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId);
    
    //Возвращает ToDoItem для UserId со статусом Active
    Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId);
    
    Task Add(ToDoItem item);
    
    Task Update(ToDoItem item);
    
    Task Delete(Guid id);
    
    //Проверяет есть ли задача с таким именем у пользователя
    Task<bool> ExistsByName(Guid userId, string name);
    
    //Возвращает количество активных задач у пользователя
    Task<int> CountActive(Guid userId);
    
    Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate); 
    
}