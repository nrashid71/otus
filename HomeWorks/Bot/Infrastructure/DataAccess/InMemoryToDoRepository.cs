namespace Bot;

public class InMemoryToDoRepository : IToDoRepository
{
    private List<ToDoItem> _toDoItems = new List<ToDoItem>();
    
    public async Task<ToDoItem?> GetByGuid(Guid id) => _toDoItems.FirstOrDefault(i => i.Id == id);

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId) =>
        _toDoItems.Where(i => i.ToDoUser.UserId == userId).ToList().AsReadOnly();

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId) =>
         _toDoItems.Where(i => i.ToDoUser.UserId == userId && i.State == ToDoItemState.Active).ToList().AsReadOnly();

    public async Task Add(ToDoItem item)
    {
        if ((await GetByGuid(item.Id)) == null)
        {
            _toDoItems.Add(item);
        }
        else
        {
            throw new Exception($"Задача с id {item.Id} уже существует");
        }
    }

    public async Task Update(ToDoItem item)
    {
        var i = await GetByGuid(item.Id);
        if (i != null)
        {
            i.State = item.State;
            i.Name = item.Name;
        }
        else
        {
            Add(item);
        }
    }

    public async Task Delete(Guid id)
    {
        var i = await GetByGuid(id);
        if (i != null)
        {
            _toDoItems.Remove(i);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name) => _toDoItems.Any(i => i.Name == name && i.ToDoUser.UserId == userId);

    public async Task<int> CountActive(Guid userId) => _toDoItems.Count(i => i.State == ToDoItemState.Active);
    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate)
    {
        return _toDoItems.Where(t => t.ToDoUser.UserId == userId && predicate(t)).ToList().AsReadOnly();
    }
}