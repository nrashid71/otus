using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public interface IUserRepository
{
    User? GetUser(Guid userId);
    User? GetUserByTelegramUserId(long telegramUserId);
    void Add(ToDoUser user);
}