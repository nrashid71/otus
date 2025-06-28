using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class AddListScenario : IScenario
{
    private readonly IUserService _userService;
    
    private readonly IToDoListService _toDoListService;

    public AddListScenario(IUserService userService, IToDoListService toDoListService)
    {
        _userService = userService;
        _toDoListService = toDoListService;
    }

    public bool CanHandle(ScenarioType scenario)
    {
        return scenario == ScenarioType.AddList;
    }

    public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
    {
        switch (context.CurrentStep)
        {
            case null:
                ToDoUser toDoUser = _userService.GetUser(update?.CallbackQuery?.From?.Id ?? 0, ct).Result;
                context.Data.Add("User", toDoUser);
                context.CurrentStep = "Name";
                await bot.SendMessage(update.CallbackQuery.Message.Chat,"Введите название списка:", cancellationToken:ct);
                return ScenarioResult.Transition;
            case "Name":
                await _toDoListService.Add((ToDoUser)context.Data["User"], update?.Message?.Text, ct);
                return ScenarioResult.Completed;
            default:
                throw new ArgumentOutOfRangeException($"Непредусмотренный к обработке шаг \"{context.CurrentStep}\"");
        }
    }
}