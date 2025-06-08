namespace Bot;

public class InMemoryScenarioContextRepository : IScenarioContextRepository
{
    private Dictionary<long, ScenarioContext>  _contexts = new Dictionary<long, ScenarioContext>();
        
    public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
    {
        return _contexts.GetValueOrDefault(userId); 
    }

    public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
    {
        if (!_contexts.ContainsKey(userId))
        {
            _contexts.Add(userId, context);
        }
    }

    public async Task ResetContext(long userId, CancellationToken ct)
    {
        if (_contexts.ContainsKey(userId))
            _contexts.Remove(userId);
    }
}
