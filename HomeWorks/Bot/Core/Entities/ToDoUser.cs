namespace Bot;

public class ToDoUser
{
    public ToDoUser(long telegramUserId, string telegramUserName)
    {
        TelegramUserId = telegramUserId;
        TelegramUserName = telegramUserName;
        UserId = Guid.NewGuid();
    }
    public Guid UserId { get; init; }
    
    public long TelegramUserId { get; init; }
    
    public string? TelegramUserName { get; init; }
    
    public DateTime RegisteredAt { get; init; }
}