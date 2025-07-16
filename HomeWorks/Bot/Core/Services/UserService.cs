namespace Bot;

public class UserService : IUserService
{
    IUserRepository _userRepository;

    public UserService(IUserRepository inMemoryUserRepository)
    {
        _userRepository = inMemoryUserRepository;
    }

    public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
    {
        var toDoUser = await GetUser(telegramUserId, ct);
        if (toDoUser == null)
        {
            toDoUser = new ToDoUser()
            {
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid()
            };
            await _userRepository.Add(toDoUser, ct);
        }
        return toDoUser;
    }

    public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct)
    {
        var result = await _userRepository.GetUserByTelegramUserId(telegramUserId, ct);
        return result;
    }
}