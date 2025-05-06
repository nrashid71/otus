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

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId)
    {
        var result = await InMemoryToDoRepository.GetAllByUserId(userId);
        return result;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId)
    {
        var result = await InMemoryToDoRepository.GetActiveByUserId(userId);
        return result;
    }

    public async Task<ToDoItem> Add(ToDoUser user, string name)
    {
        ToDoItem toDoItem = new ToDoItem(name, user);
        
        await InMemoryToDoRepository.Add(toDoItem);
        
        return toDoItem;
    }

    public async Task MarkCompleted(Guid id)
    {
        var task = await ((InMemoryToDoRepository)InMemoryToDoRepository).GetByGuid(id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
            // InMemoryToDoRepository.Update(task);
        }
    }
    
    public async Task Delete(Guid id)
    {
        await InMemoryToDoRepository.Delete(id);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix)
    {
        var r = await InMemoryToDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix));
        return r.ToList().AsReadOnly();
    }
}


