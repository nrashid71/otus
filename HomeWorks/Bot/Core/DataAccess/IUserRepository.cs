using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public interface IUserRepository
{
    Task<ToDoUser?> GetUser(Guid userId);
    Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId);
    Task Add(ToDoUser user);
}