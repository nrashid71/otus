namespace Bot;

public class ScenarioContext
{
    public long UserId { get; } //Id пользователя в Telegram
    
    public ScenarioType CurrentScenario { get; }
    
    public string? CurrentStep { set; get; }
    
    public Dictionary<string, object> Data { set; get; }

    public DateTime CreatedAt { get; } 
    public ScenarioContext(ScenarioType scenario, long userId)
    {
        Data = new Dictionary<string, object>();
        CurrentScenario = scenario;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

}