namespace Bot;

public class UserService : IUserService
{
    InMemoryUserRepository _userRepository = new InMemoryUserRepository();
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        var toDoUser = GetUser(telegramUserId);
        if (toDoUser == null)
        {
            toDoUser = new ToDoUser(telegramUserId, telegramUserName);
            _userRepository.Add(toDoUser);
        }
        return toDoUser;
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        return _userRepository.GetToDoUserByTelegramUserId(telegramUserId);
    }
}