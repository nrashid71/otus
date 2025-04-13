using Otus.ToDoList.ConsoleBot.Types; 
    
namespace Bot;

public class ToDoUser
{
    public User? User { get; set; }
    public ToDoUser(long telegramUserId, string telegramUserName)
    {
        TelegramUserId = telegramUserId;
        TelegramUserName = telegramUserName;
    }

    public ToDoUser(User user)
    {
        TelegramUserId = user.Id;
        TelegramUserName = user.Username;
        User = user;
    }

    public Guid UserId { get; init; }
    
    public long TelegramUserId { get; init; }
    
    public string? TelegramUserName { get; init; }
    
    public DateTime RegisteredAt { get; init; }
}