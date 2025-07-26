namespace Bot;

public class ToDoItem
{
    private ToDoItemState _state;
    public Guid Id { get; set; }
    public ToDoUser ToDoUser { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Deadline { get; set; }
    public ToDoList? List { get; set; }
    public ToDoItemState State { get; set; }
    public DateTime? StateChangedAt { get; set; }
    
}