namespace Bot;
public class ToDoService : IToDoService
{
    /// <summary>
    /// Максимальная длина задачи, указанная пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    private readonly int _taskLengthLimit = 10;

    /// <summary>
    /// Максимальное количество задач, указанное пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    private readonly int _taskCountLimit = 100;

    private readonly IToDoRepository _toDoRepository;

    public ToDoService(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId)
    {
        var result = await _toDoRepository.GetAllByUserId(userId);
        return result;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId)
    {
        var result = await _toDoRepository.GetActiveByUserId(userId);
        return result;
    }

    public async Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline)
    {
        
        if (name.Length > _taskLengthLimit)
        {
            throw new TaskLengthLimitException(name.Length, _taskLengthLimit);
        }

        if (await _toDoRepository.ExistsByName(user.UserId, name))
        {
            throw new DuplicateTaskException(name);
        }        

        if ((await GetAllByUserId(user.UserId)).Count(t => t.State == ToDoItemState.Active) >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }        
        
        ToDoItem toDoItem = new ToDoItem(name, user, deadline);
        
        await _toDoRepository.Add(toDoItem);
        
        return toDoItem;
    }

    public async Task MarkCompleted(Guid id)
    {
        var task = await _toDoRepository.GetByGuid(id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
            _toDoRepository.Update(task);
        }
    }
    
    public async Task Delete(Guid id)
    {
        await _toDoRepository.Delete(id);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix)
    {
        var r = await _toDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix));
        return r.ToList().AsReadOnly();
    }
}


