namespace Bot;

public interface IUserService
{
    Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct);
    
    Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct);
    
}