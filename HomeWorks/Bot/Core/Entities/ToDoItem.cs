namespace Bot;

public class ToDoItem
{
    private ToDoItemState _state;
    public Guid Id { get; init; }
    public ToDoUser ToDoUser { get; init; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime Deadline { set; get; }
    
    public ToDoList? List { set; get; }
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

    public ToDoItem(string name, ToDoUser toDoUser, DateTime deadline, ToDoList? list)
    {
        Id = Guid.NewGuid();
        ToDoUser = toDoUser;
        Name = name;
        CreatedAt = DateTime.Now;
        State = ToDoItemState.Active;
        Deadline = deadline;
        List = list;
    }
}