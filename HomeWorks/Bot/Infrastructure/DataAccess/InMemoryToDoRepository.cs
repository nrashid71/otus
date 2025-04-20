namespace Bot;

public class InMemoryToDoRepository : IToDoRepository
{
    private List<ToDoItem> _toDoItems = new List<ToDoItem>();
    
    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId) =>
        _toDoItems.Where(i => i.ToDoUser.UserId == userId).ToList().AsReadOnly();

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId) =>
         _toDoItems.Where(i => i.ToDoUser.UserId == userId && i.State == ToDoItemState.Active).ToList().AsReadOnly();

    public void Add(ToDoItem item)
    {
        if (GetByGuid(item.Id) == null)
        {
            _toDoItems.Add(item);
        }
        else
        {
            throw new Exception($"Задача с id {item.Id} уже существует");
        }
    }

    public void Update(ToDoItem item)
    {
        var i = GetByGuid(item.Id);
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

    public void Delete(Guid id)
    {
        var i = GetByGuid(id);
        if (i != null)
        {
            _toDoItems.Remove(i);
        }
    }

    public bool ExistsByName(Guid userId, string name) => _toDoItems.Any(i => i.Name == name);

    public int CountActive(Guid userId) => _toDoItems.Count(i => i.State == ToDoItemState.Active);
    public ToDoItem? GetByGuid(Guid id) => _toDoItems.FirstOrDefault(i => i.Id == id);
    public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
    {
        return _toDoItems.Where(t => t.ToDoUser.UserId == userId && predicate(t)).ToList().AsReadOnly();
    }
}