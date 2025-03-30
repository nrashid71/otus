namespace Bot;

public interface IUserService
{
    User RegisterUser(long telegramUserId, string telegramUserName);
    
    User? GetUser(long telegramUserId);
    
}