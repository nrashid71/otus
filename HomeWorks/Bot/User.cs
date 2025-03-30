namespace Bot;

public class User
{
    public User(long telegramUserId, string telegramUserName)
    {
        TelegramUserId = telegramUserId;
        TelegramUserName = telegramUserName;
    }

    public Guid UserId { get; init; }
    
    public long TelegramUserId { get; init; }
    
    public string? TelegramUserName { get; init; }
    
    public DateTime RegisteredAt { get; init; }
}