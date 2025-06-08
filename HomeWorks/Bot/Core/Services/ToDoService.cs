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
    
    private IToDoRepository ToDoRepository { get; }

    public ToDoService(IToDoRepository toDoRepository)
    {
        ToDoRepository = toDoRepository;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId)
    {
        var result = await ToDoRepository.GetAllByUserId(userId);
        return result;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId)
    {
        var result = await ToDoRepository.GetActiveByUserId(userId);
        return result;
    }

    public async Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline)
    {
        
        if (name.Length > _taskLengthLimit)
        {
            throw new TaskLengthLimitException(name.Length, _taskLengthLimit);
        }

        if ((await ToDoRepository.GetAllByUserId(user.UserId)).Any(t => t.Name == name))
        {
            throw new DuplicateTaskException(name);
        }        

        if ((await GetAllByUserId(user.UserId)).Count >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }        
        
        ToDoItem toDoItem = new ToDoItem(name, user, deadline);
        
        await ToDoRepository.Add(toDoItem);
        
        return toDoItem;
    }

    public async Task MarkCompleted(Guid id)
    {
        var task = await ToDoRepository.GetByGuid(id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
            ToDoRepository.Update(task);
        }
    }
    
    public async Task Delete(Guid id)
    {
        await ToDoRepository.Delete(id);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix)
    {
        var r = await ToDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix));
        return r.ToList().AsReadOnly();
    }
}


