using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class AddTaskScenario : IScenario
{
    /// <summary>
    /// Максимальная длина задачи, указанная пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    private readonly int _taskLengthLimit = 1000;

    private IUserService _userService;
    
    private IToDoService _toDoService;

    public ScenarioType ScenarioType { set; get; }

    public AddTaskScenario(ScenarioType scenarioType, IUserService userService, IToDoService toDoService)
    {
        ScenarioType = scenarioType;
        _userService = userService;
        _toDoService = toDoService;
    }

    public bool CanHandle(ScenarioType scenario)
    {
        return scenario == ScenarioType;
    }

    public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
    {
        ScenarioResult result = default(ScenarioResult);
        switch (context.CurrentStep)
        {
            case null:
                context.UserId = update?.Message?.From?.Id ?? 0;
                await bot.SendMessage(update.Message.Chat,"Введите название задачи:", cancellationToken:ct);
                context.CurrentStep = "Name";
                result = ScenarioResult.Transition;
                break;
            case "Name":
                string description = update.Message.Text;
                if (!string.IsNullOrEmpty(description))
                {
                    ToDoUser toDoUser = _userService.GetUser(update?.Message?.From?.Id ?? 0).Result;
                    if (description.Length > _taskLengthLimit)
                    {
                        throw new TaskLengthLimitException(description.Length, _taskLengthLimit);
                    }

                    if ((await _toDoService.GetAllByUserId(toDoUser.UserId)).Any(t => t.Name == description))
                    {
                        throw new DuplicateTaskException(description);
                    }

                    _toDoService.Add(toDoUser, description);

                    await bot.SendMessage(update.Message.Chat,"Задача добавлена.", cancellationToken:ct);
                    
                    return ScenarioResult.Completed;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException($"Непредусмотренный к обработке шаг \"{context.CurrentStep}\"");
        }
        return result;
    }
}