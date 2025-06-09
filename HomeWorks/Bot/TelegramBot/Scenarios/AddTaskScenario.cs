using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

public class AddTaskScenario : IScenario
{

    private readonly IUserService _userService;
    
    private readonly IToDoService _toDoService;

    public AddTaskScenario(IUserService userService, IToDoService toDoService)
    {
        _userService = userService;
        _toDoService = toDoService;
    }

    public bool CanHandle(ScenarioType scenario)
    {
        return scenario == ScenarioType.AddTask;
    }

    public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
    {
        ScenarioResult result = default(ScenarioResult);
        ReplyKeyboardMarkup replyMarkup;
        switch (context.CurrentStep)
        {
            case null:
                replyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "/cancel",}
                }) 
                {
                    ResizeKeyboard = true,
                };
                
                await bot.SendMessage(update.Message.Chat,"Введите название задачи:", cancellationToken:ct, replyMarkup: replyMarkup);
                
                context.CurrentStep = "Name";
                
                result = ScenarioResult.Transition;
                
                break;
            case "Name":
                string name = update.Message.Text;
                
                if (!string.IsNullOrEmpty(name))
                {
                    context.Data.Add("Name", name);

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

                    await bot.SendMessage(update.Message.Chat,"Задача добавлена.", cancellationToken:ct, replyMarkup: KeyboardHelper.GetDefaultKeyboard());
                    
                    return ScenarioResult.Completed;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException($"Непредусмотренный к обработке шаг \"{context.CurrentStep}\"");
        }
        return result;
    }
}