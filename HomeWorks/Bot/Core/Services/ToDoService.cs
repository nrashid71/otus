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

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        var result = await _toDoRepository.GetAllByUserId(userId,ct);
        return result;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        var result = await _toDoRepository.GetActiveByUserId(userId,ct);
        return result;
    }

    public async Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline, ToDoList? list, CancellationToken ct)
    {
        
        if (name.Length > _taskLengthLimit)
        {
            throw new TaskLengthLimitException(name.Length, _taskLengthLimit);
        }

        if (await _toDoRepository.ExistsByName(user.UserId, name,ct))
        {
            throw new DuplicateTaskException(name);
        }        

        if ((await GetAllByUserId(user.UserId,ct)).Count(t => t.State == ToDoItemState.Active) >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }        
        
        ToDoItem toDoItem = new ToDoItem(name, user, deadline, list);
        
        await _toDoRepository.Add(toDoItem,ct);
        
        return toDoItem;
    }
    public async Task MarkCompleted(Guid id, CancellationToken ct)
    {
        var task = await _toDoRepository.GetByGuid(id,ct);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
            _toDoRepository.Update(task,ct);
        }
    }
    
    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _toDoRepository.Delete(id,ct);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
    {
        var r = await _toDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix),ct);
        return r.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct)
    {
        return GetAllByUserId(userId,ct).Result.Where(t => t.List?.Id == listId).ToList().AsReadOnly();
    }
    public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct)
    {
        return await _toDoRepository.GetByGuid(toDoItemId,ct);        
    }
}


