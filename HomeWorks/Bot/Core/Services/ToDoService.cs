namespace Bot;
public class ToDoService : IToDoService
{
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


