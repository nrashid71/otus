using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
        ReplyKeyboardMarkup replyMarkup;
        switch (context.CurrentStep)
        {
            case null:
                context.UserId = update?.Message?.From?.Id ?? 0;
                replyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "/cancel","/addtask","/showalltasks", "/showtasks", "/report" }
                }) 
                {
                    ResizeKeyboard = true,
                };
                await bot.SendMessage(update.Message.Chat,"Введите название задачи:", cancellationToken:ct, replyMarkup: replyMarkup);
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
                        await bot.SendMessage(update.Message.Chat,
                                            $"Длина описания задачи {description.Length} превышает максимально допустимое значение {_taskLengthLimit}",
                                            cancellationToken:ct);
                        return ScenarioResult.Transition;
                    }

                    if ((await _toDoService.GetAllByUserId(toDoUser.UserId)).Any(t => t.Name == description))
                    {
                        await bot.SendMessage(update.Message.Chat,
                            $"Задача \"{description}\" уже существует.",
                            cancellationToken:ct);
                        return ScenarioResult.Transition;
                    }

                    context.Data.Add("Name", description);
                    context.CurrentStep = "Deadline";
                    await bot.SendMessage(update.Message.Chat,"Введите дату завершения задачи:", cancellationToken:ct);
                    
                    return ScenarioResult.Transition;
                }

                break;
            case "Deadline":
                string deadline = update.Message.Text;
                if (!string.IsNullOrEmpty(deadline))
                {
                    ToDoUser toDoUser = _userService.GetUser(update?.Message?.From?.Id ?? 0).Result;

                    if (!DateTime.TryParse(deadline, out DateTime deadlineDate))
                    {
                        await bot.SendMessage(update.Message.Chat,"Дата ожидается в формате dd.MM.yyyy", cancellationToken:ct);
                    
                        return ScenarioResult.Transition;
                    }
                    
                    _toDoService.Add(toDoUser, (string)context.Data["Name"], deadlineDate);

                    replyMarkup  = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "/addtask","/showalltasks", "/showtasks", "/report" }
                    }) 
                    {
                        ResizeKeyboard = true,
                    };
                    
                    await bot.SendMessage(update.Message.Chat,"Задача добавлена.", cancellationToken:ct, replyMarkup: replyMarkup);
                    
                    return ScenarioResult.Completed;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException($"Непредусмотренный к обработке шаг \"{context.CurrentStep}\"");
        }
        return result;
    }
}