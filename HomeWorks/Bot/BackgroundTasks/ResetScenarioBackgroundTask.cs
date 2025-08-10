using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class ResetScenarioBackgroundTask : BackgroundTask
{
    private readonly IScenarioContextRepository _scenarioRepository;
    
    private readonly ITelegramBotClient _botClient;

    private readonly TimeSpan _resetScenarioTimeout;
    
    public  ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository scenarioRepository, ITelegramBotClient bot) : base(resetScenarioTimeout, nameof(ResetScenarioBackgroundTask))
    {
        _scenarioRepository = scenarioRepository;
        _botClient = bot;
        _resetScenarioTimeout = resetScenarioTimeout;
    }
    protected override async Task Execute(CancellationToken ct)
    {
        foreach (var s in await _scenarioRepository.GetContexts(ct))
        {
            if (DateTime.UtcNow - s.CreatedAt > _resetScenarioTimeout)
            {
                await _scenarioRepository.ResetContext(s.UserId, ct);
                await _botClient.SendMessage(((Chat)s.Data["Chat"]).Id,
                    $"Сценарий отменен, так как не поступил ответ в течение {_resetScenarioTimeout}",
                    cancellationToken:ct,
                    replyMarkup: KeyboardHelper.GetDefaultKeyboard());
            }
        }
    }
}