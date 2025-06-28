using System.Text.Json;

namespace Bot;

public class FileToDoRepository : IToDoRepository
{
    private string _baseDirecory;

    private string _indexFile;
    
    public FileToDoRepository(string baseFolder)
    {
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
        _baseDirecory = baseFolder;

        _indexFile = Path.Combine(_baseDirecory, "index.json");
            
        if (!File.Exists(_indexFile))
        {
            CreateFileIndex();
        }
    }
    private async Task WriteFileIndex(List<Index>  indexes)
    {
        await File.WriteAllTextAsync(_indexFile, JsonSerializer.Serialize<List<Index>>(indexes, new JsonSerializerOptions { WriteIndented = true }));
    }
    private async Task<List<Index>> ReadFileIndex()
    {
        if (!File.Exists(_indexFile))
        {
            await CreateFileIndex();
        }        
        return JsonSerializer.Deserialize<List<Index>>(File.ReadAllText(_indexFile));
    }
    private async Task CreateFileIndex()
    {
        await WriteFileIndex(
            Directory.GetFiles(_baseDirecory, searchOption: SearchOption.AllDirectories, searchPattern: "*.json")
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
                .Where(item => item != null)
                .Select(item => new Index(item.ToDoUser.UserId, item.Id)).ToList());
    }
    public async Task<ToDoItem?> GetByGuid(Guid id, CancellationToken ct)
    {
        return (await Find(
            (await ReadFileIndex()).FirstOrDefault(record => record.ItemId == id)?.UserId ?? Guid.Empty,
            item => item.Id == id, ct)).FirstOrDefault();
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        var userFolder = Path.Combine(_baseDirecory, userId.ToString());
        if (!Directory.Exists(userFolder))
        {
            return new List<ToDoItem>().AsReadOnly();
        }
        else
        {
            return Directory.EnumerateFiles(userFolder)
                .Where(file => Path.GetExtension(file) == ".json")
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
                .Where(item => item != null && item.ToDoUser.UserId == userId)
                .ToList()
                .AsReadOnly();
        }
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        return (await GetAllByUserId(userId, ct)).Where(i => i.State == ToDoItemState.Active).ToList().AsReadOnly();
    }

    public async Task Add(ToDoItem item, CancellationToken ct)
    {
        var index = await ReadFileIndex();
        var userFolder = Path.Combine(_baseDirecory, item.ToDoUser.UserId.ToString());
        if (!Directory.Exists(userFolder))
        {
            Directory.CreateDirectory(userFolder);
        }
        var filePath = Path.Combine(userFolder, $"{item.Id}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }));
        index.Add(new Index(item.ToDoUser.UserId, item.Id));
        await WriteFileIndex(index);
    }

    public async Task Update(ToDoItem item, CancellationToken ct)
    {
        var filePath = Path.Combine(_baseDirecory, item.ToDoUser.UserId.ToString(), $"{item.Id.ToString()}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }));   
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var indexes = (await ReadFileIndex());
        var index = indexes.FirstOrDefault(record => record.ItemId == id);
        if (index != null)
        {
            indexes.Remove(index);
            var filePath = Path.Combine(_baseDirecory, index.UserId.ToString(), $"{id.ToString()}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        return (await GetAllByUserId(userId, ct)).Any(i => i.Name == name);
    }

    public async Task<int> CountActive(Guid userId, CancellationToken ct)
    {
        return (await GetAllByUserId(userId, ct)).Count();
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
    {
        return (await GetAllByUserId(userId, ct)).Where(t => predicate(t)).ToList().AsReadOnly();
    }
}