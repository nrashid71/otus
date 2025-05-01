using System.Collections.Immutable;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;
public class ToDoService : IToDoService
{
    private IToDoRepository InMemoryToDoRepository { get; }

    public ToDoService(IToDoRepository inMemoryToDoRepository)
    {
        InMemoryToDoRepository = inMemoryToDoRepository;
    }

    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return InMemoryToDoRepository.GetAllByUserId(userId);
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return InMemoryToDoRepository.GetActiveByUserId(userId);
    }

    public ToDoItem Add(ToDoUser user, string name)
    {
        ToDoItem toDoItem = new ToDoItem(name, user);
        
        InMemoryToDoRepository.Add(toDoItem);
        
        return toDoItem;
    }

    public void MarkCompleted(Guid id)
    {
        var task = ((InMemoryToDoRepository)InMemoryToDoRepository).GetByGuid(id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
            // InMemoryToDoRepository.Update(task);
        }
    }
    
    public void Delete(Guid id)
    {
        InMemoryToDoRepository.Delete(id);
    }
    
    public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix) =>
         InMemoryToDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix)).ToList().AsReadOnly();
    
}


