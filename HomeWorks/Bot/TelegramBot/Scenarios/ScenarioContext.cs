namespace Bot;

public class ScenarioContext
{
    public long UserId { get; } //Id пользователя в Telegram
    
    public ScenarioType CurrentScenario { get; }
    
    public string? CurrentStep { set; get; }
    
    public Dictionary<string, object> Data { set; get; }

    public ScenarioContext(ScenarioType scenario, long userId)
    {
        Data = new Dictionary<string, object>();
        CurrentScenario = scenario;
        UserId = userId;
    }

}