namespace Bot;

public class InMemoryUserRepository : IUserRepository
{
    private List<ToDoUser> _users = new List<ToDoUser>();

    public async Task<ToDoUser?> GetUser(Guid userId)
    {
        return _users.FirstOrDefault(u => u.UserId == userId);
    }

    public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId)
    {
        return _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
    }

    public async Task Add(ToDoUser user)
    {
        _users.Add(user);
    }
}