namespace Bot;

public class UserService : IUserService
{
    IUserRepository _userRepository;

    public UserService(IUserRepository inMemoryUserRepository)
    {
        _userRepository = inMemoryUserRepository;
    }

    public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName)
    {
        var toDoUser = await GetUser(telegramUserId);
        if (toDoUser == null)
        {
            toDoUser = new ToDoUser(telegramUserId, telegramUserName);
            await _userRepository.Add(toDoUser);
        }
        return toDoUser;
    }

    public async Task<ToDoUser?> GetUser(long telegramUserId)
    {
        var result = await _userRepository.GetUserByTelegramUserId(telegramUserId);
        return result;
    }
}