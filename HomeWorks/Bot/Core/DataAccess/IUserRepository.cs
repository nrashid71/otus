using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public interface IUserRepository
{
    ToDoUser? GetUser(Guid userId);
    ToDoUser? GetUserByTelegramUserId(long telegramUserId);
    void Add(ToDoUser user);
}