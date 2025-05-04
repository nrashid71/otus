namespace Bot;

public interface IUserService
{
    Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName);
    
    Task<ToDoUser?> GetUser(long telegramUserId);
    
}