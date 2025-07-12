namespace Bot;

public class ToDoList
{
    public Guid Id  { get; set; }

    public string Name { get; set; }
    
    public ToDoUser ToDoUser { get; set; }
    
    public DateTime CreatedAt { get; set; }
}