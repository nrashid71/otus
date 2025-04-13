namespace Bot;

public class UserService : IUserService
{
    InMemoryUserRepository _userRepository = new InMemoryUserRepository();
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        return GetUser(telegramUserId) ?? new ToDoUser(telegramUserId, telegramUserName);
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        return _userRepository.GetToDoUserByTelegramUserId(telegramUserId);
    }
}