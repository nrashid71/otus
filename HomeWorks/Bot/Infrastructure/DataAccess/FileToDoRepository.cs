using System.Text.Json;

namespace Bot;

public class FileToDoRepository : IToDoRepository
{
    private string _baseDirecory;
    
    public FileToDoRepository(string baseFolder)
    {
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
        _baseDirecory = baseFolder;
    }

    private async Task<IEnumerable<ToDoItem>> GetAll()
    {
        return Directory.EnumerateFiles(_baseDirecory)
            .ToList()
            .Select((file) =>
            {
                try
                {
                    return JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error proccesing file {file}:\n{ex.Message}");
                    return null;
                }
            })
            .Where(item => item != null);
    }

    public async Task<ToDoItem?> GetByGuid(Guid id)
    {
        return (await GetAll()).FirstOrDefault(item => item.Id == id);
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId)
    {
        return (await GetAll()).Where(item => item.ToDoUser.UserId == userId).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId)
    {
        return (await GetAllByUserId(userId)).Where(i => i.State == ToDoItemState.Active).ToList().AsReadOnly();
    }

    public async Task Add(ToDoItem item)
    {
        var filePath = Path.Combine(_baseDirecory, item.Id.ToString());
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item));   
    }

    public async Task Update(ToDoItem item)
    {
        var filePath = Path.Combine(_baseDirecory, item.Id.ToString());
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item));   
    }

    public async Task Delete(Guid id)
    {
        var filePath = Path.Combine(_baseDirecory, id.ToString());
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name)
    {
        return (await GetAllByUserId(userId)).Any(i => i.Name == name);
    }

    public async Task<int> CountActive(Guid userId)
    {
        return (await GetAllByUserId(userId)).Count();
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate)
    {
        return (await GetAllByUserId(userId)).Where(t => predicate(t)).ToList().AsReadOnly();
    }
}