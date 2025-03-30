namespace Bot;

public class UserService : IUserService
{
    public User RegisterUser(long telegramUserId, string telegramUserName)
    {
        return new User(telegramUserId, telegramUserName);
    }

    public User? GetUser(long telegramUserId)
    {
        throw new NotImplementedException();
    }
}