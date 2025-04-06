namespace Bot;

public class UserService : IUserService
{
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        return new ToDoUser(telegramUserId, telegramUserName);
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        throw new NotImplementedException();
    }
}