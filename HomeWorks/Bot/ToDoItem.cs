namespace Bot;

public class ToDoItem
{
    private ToDoItemState _state;
    public Guid Id { get; init; }
    public User User { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }

    public ToDoItemState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                StateChangedAt = DateTime.Now;
            }
            _state = value;
        }
    }
    public DateTime? StateChangedAt { get; set; }

    public ToDoItem(string name, User user)
    {
        Id = Guid.NewGuid();
        User = user;
        Name = name;
        CreatedAt = DateTime.Now;
        State = ToDoItemState.Active;
    }
}