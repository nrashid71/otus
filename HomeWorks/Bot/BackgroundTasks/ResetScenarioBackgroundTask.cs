using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class ResetScenarioBackgroundTask : BackgroundTask
{
    private IScenarioContextRepository _scenarioRepository;
    
    private ITelegramBotClient _botClient;

    private TimeSpan _resetScenarioTimeout;
    
    public  ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository scenarioRepository, ITelegramBotClient bot) : base(resetScenarioTimeout, nameof(ResetScenarioBackgroundTask))
    {
        _scenarioRepository = scenarioRepository;
        _botClient = bot;
        _resetScenarioTimeout = resetScenarioTimeout;
    }
    protected override async Task Execute(CancellationToken ct)
    {
        foreach (var s in _scenarioRepository.GetContexts(ct).Result)
        {
            if (DateTime.UtcNow - s.CreatedAt > _resetScenarioTimeout)
            {
                _scenarioRepository.ResetContext(s.UserId, ct);
                await _botClient.SendMessage(((Chat)s.Data["Chat"]).Id,
                    $"Сценарий отменен, так как не поступил ответ в течение {_resetScenarioTimeout}",
                    cancellationToken:ct,
                    replyMarkup: KeyboardHelper.GetDefaultKeyboard());
            }
        }
    }
}