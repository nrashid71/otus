using System.Text.Json;

namespace Bot;
public class FileUserRepository : IUserRepository
{
    private string _baseDirecory;
    
    public FileUserRepository(string baseFolder)
    {
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
        _baseDirecory = baseFolder;
    }

    private async Task<IEnumerable<ToDoUser>> GetAll()
    {
        return Directory.EnumerateFiles(_baseDirecory)
            .Where(file => Path.GetExtension(file) == ".json")
            .Select((file) =>
            {
                try
                {
                    return JsonSerializer.Deserialize<ToDoUser>(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error proccesing file {file}:\n{ex.Message}");
                    return null;
                }
            })
            .Where(user => user != null);
    }
    
    public async Task<ToDoUser?> GetUser(Guid userId)
    {
        return (await GetAll()).FirstOrDefault(user => user.UserId == userId);        
    }

    public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId)
    {
        return (await GetAll()).FirstOrDefault(user => user.TelegramUserId == telegramUserId);        
    }

    public async Task Add(ToDoUser user)
    {
        var filePath = Path.Combine(_baseDirecory, $"{user.UserId}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true }));           
    }
}