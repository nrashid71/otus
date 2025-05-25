namespace Bot;

public class ScenarioContext
{
    public long UserId { set; get; } //Id пользователя в Telegram
    
    public ScenarioType CurrentScenario { set; get; }
    
    public string? CurrentStep { set; get; }
    
    public Dictionary<string, object> Data { set; get; }

    public ScenarioContext(ScenarioType scenario)
    {
        CurrentScenario = scenario;
    }

}