namespace Bot;

public class InMemoryToDoRepository : IToDoRepository
{
    private List<ToDoItem> _toDoItems = new List<ToDoItem>();
    
    public async Task<ToDoItem?> GetByGuid(Guid id, CancellationToken ct) => _toDoItems.FirstOrDefault(i => i.Id == id);

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct) =>
        _toDoItems.Where(i => i.ToDoUser.UserId == userId).ToList().AsReadOnly();

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct) =>
         _toDoItems.Where(i => i.ToDoUser.UserId == userId && i.State == ToDoItemState.Active).ToList().AsReadOnly();

    public async Task Add(ToDoItem item, CancellationToken ct)
    {
        if ((await GetByGuid(item.Id, ct)) == null)
        {
            _toDoItems.Add(item);
        }
        else
        {
            throw new Exception($"Задача с id {item.Id} уже существует");
        }
    }

    public async Task Update(ToDoItem item, CancellationToken ct)
    {
        var i = await GetByGuid(item.Id, ct);
        if (i != null)
        {
            i.State = item.State;
            i.Name = item.Name;
        }
        else
        {
            Add(item, ct);
        }
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var i = await GetByGuid(id, ct);
        if (i != null)
        {
            _toDoItems.Remove(i);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct) => _toDoItems.Any(i => i.Name == name && i.ToDoUser.UserId == userId);

    public async Task<int> CountActive(Guid userId, CancellationToken ct) => _toDoItems.Count(i => i.State == ToDoItemState.Active);
    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
    {
        return _toDoItems.Where(t => t.ToDoUser.UserId == userId && predicate(t)).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveWithDeadline(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}