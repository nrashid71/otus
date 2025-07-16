namespace Bot;

public class ToDoListService : IToDoListService
{
    private readonly IToDoListRepository _toDoListRepository;

    public ToDoListService(IToDoListRepository toDoListRepository)
    {
        _toDoListRepository = toDoListRepository;
    }
    public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
    {
        if (name.Length > 10)
        {
            throw new ArgumentOutOfRangeException($"Размер названия списка \"{name}\" превышает 10 символов");
        }
        if (_toDoListRepository.ExistsByName(user.UserId, name, ct).Result)
        {
            throw new ArgumentOutOfRangeException($"Список с названием \"{name}\" уже существует.");
        }

        var toDoList = new ToDoList()
        {
            Name = name,
            ToDoUser = user,
            Id = Guid.NewGuid()
        };
        _toDoListRepository.Add(toDoList, ct);
        return toDoList;
    }

    public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
    {
        return await _toDoListRepository.Get(id, ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _toDoListRepository.Delete(id, ct);
    }

    public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
    {
        return await _toDoListRepository.GetByUserId(userId, ct);
    }
}