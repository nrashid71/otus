namespace Bot;

public class ToDoList
{
    public ToDoList(string name, ToDoUser toDoUser)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Имя списка обязательно");
        }
        Id = Guid.NewGuid();
        Name = name;
        ToDoUser = toDoUser;
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; init; }

    public string Name { get; init; }
    
    public ToDoUser ToDoUser { get; init; }
    
    public DateTime CreatedAt { get; init; }
}