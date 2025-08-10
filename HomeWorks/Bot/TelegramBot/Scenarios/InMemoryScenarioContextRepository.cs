using System.Collections.Concurrent;

namespace Bot;

public class InMemoryScenarioContextRepository : IScenarioContextRepository
{
    private ConcurrentDictionary<long, ScenarioContext>  _contexts = new ConcurrentDictionary<long, ScenarioContext>();
        
    public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
    {
        return _contexts.GetValueOrDefault(userId); 
    }

    public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
    {
        if (!_contexts.ContainsKey(userId))
        {
            _contexts.TryAdd(userId, context);
        }
    }

    public async Task ResetContext(long userId, CancellationToken ct)
    {
            _contexts.Remove(userId, out _);
    }

    public async Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken ct)
    {
        return _contexts.Values.ToList().AsReadOnly();
    }
}
