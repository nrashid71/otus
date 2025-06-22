using System.Text.Json;

namespace Bot;

public class FileToDoListRepository : IToDoListRepository
{
    private string _baseDirecory;

    public FileToDoListRepository(string baseFolder)
    {
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
        _baseDirecory = baseFolder;

    }

    public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
    {
        return Directory.GetFiles(_baseDirecory, $"{id}.json", SearchOption.AllDirectories)
            .Select((file) =>
            {
                try
                {
                    return JsonSerializer.Deserialize<ToDoList>(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error proccesing file {file}:\n{ex.Message}");
                    return null;
                }
            })
            .FirstOrDefault(item => item != null && item.Id == id);
    }

    public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
    {
        var userFolder = Path.Combine(_baseDirecory, userId.ToString());
        if (!Directory.Exists(userFolder))
        {
            return new List<ToDoList>().AsReadOnly();
        }
        else
        {
            return Directory.EnumerateFiles(userFolder)
                .Where(file => Path.GetExtension(file) == ".json")
                .Select((file) =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ToDoList>(File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error proccesing file {file}:\n{ex.Message}");
                        return null;
                    }
                })
                .Where(item => item != null && item.ToDoUser.UserId == userId)
                .ToList()
                .AsReadOnly();
        }        
        
    }

    public async Task Add(ToDoList list, CancellationToken ct)
    {
        var userFolder = Path.Combine(_baseDirecory, list.ToDoUser.UserId.ToString());
        if (!Directory.Exists(userFolder))
        {
            Directory.CreateDirectory(userFolder);
        }
        var filePath = Path.Combine(userFolder, $"{list.Id}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var toDoList = await Get(id, ct);
        var filePath = Path.Combine(_baseDirecory, toDoList.ToDoUser.UserId.ToString(), $"{id.ToString()}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        return (await GetByUserId(userId, ct)).Any(i => i.Name == name);
    }
}