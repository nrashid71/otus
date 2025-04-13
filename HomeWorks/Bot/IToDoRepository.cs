namespace Bot;

public interface IToDoRepository
{
    IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
    
    //Возвращает ToDoItem для UserId со статусом Active
    IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
    
    void Add(ToDoItem item);
    
    void Update(ToDoItem item);
    
    void Delete(Guid id);
    
    //Проверяет есть ли задача с таким именем у пользователя
    bool ExistsByName(Guid userId, string name);
    
    //Возвращает количество активных задач у пользователя
    int CountActive(Guid userId);
    
}