namespace Bot;

public interface IUserService
{
    ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
    
    ToDoUser? GetUser(long telegramUserId);
    
}