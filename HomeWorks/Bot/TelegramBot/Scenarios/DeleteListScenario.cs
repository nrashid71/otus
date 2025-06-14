using Bot.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

public class DeleteListScenario : IScenario
{
    private readonly IUserService _userService;
    
    private readonly IToDoListService _toDoListService;

    private readonly IToDoService _toDoService;

    public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
    {
        _userService = userService;
        _toDoListService = toDoListService;
        _toDoService = toDoService;
    }
    public bool CanHandle(ScenarioType scenario)
    {
        return scenario == ScenarioType.DeleteList;
    }

    public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
    {
        switch (context.CurrentStep)
        {
            case null:
                ToDoUser toDoUser = _userService.GetUser(update?.CallbackQuery?.From?.Id ?? 0).Result;
                context.Data.Add("User", toDoUser);
                var inlineKeyboard = new InlineKeyboardMarkup(
                    _toDoListService.GetUserLists(toDoUser.UserId, ct).Result.Select(
                             l => new[]{InlineKeyboardButton.WithCallbackData(l.Name, "deletelist|" + l.Id)}));
                context.CurrentStep = "Approve";
                await bot.SendMessage(update.CallbackQuery.Message.Chat.Id,
                    "Выберите список для удаления",
                    cancellationToken:ct,
                    replyMarkup: inlineKeyboard);
                return ScenarioResult.Transition;
            case "Approve":
                var toDoListCallbackDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                context.Data.Add("ToDoList", toDoListCallbackDto.ToDoListId);
                var toDoList = _toDoListService.Get(toDoListCallbackDto.ToDoListId ?? Guid.Empty, ct).Result;
                context.CurrentStep = "Delete";
                await bot.SendMessage(update.CallbackQuery.Message.Chat.Id,
                    $"Подтверждаете удаление списка {toDoList?.Name} и всех его задач",
                    cancellationToken:ct,
                    replyMarkup: new InlineKeyboardMarkup(
                        new[]{
                            new[]{
                                    InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                                    InlineKeyboardButton.WithCallbackData("❌Нет", "no")
                            }
                        })
                    );
                return ScenarioResult.Transition;
            case "Delete":
                var callbackDto = CallbackDto.FromString(update.CallbackQuery.Data);
                if (callbackDto.Action == "yes")
                {
                    var toDoListId = (Guid)context.Data["ToDoList"];
                    foreach (var t in _toDoService
                                 .GetByUserIdAndList(((ToDoUser)context.Data["User"]).UserId, toDoListId, ct).Result)
                    {
                        await _toDoService.Delete(t.Id);
                    }
                    await _toDoListService.Delete(toDoListId, ct);
                    await bot.SendMessage(update.CallbackQuery.Message.Chat,"Список удален", cancellationToken:ct, replyMarkup: KeyboardHelper.GetDefaultKeyboard());
                }
                else
                {
                    await bot.SendMessage(update.CallbackQuery.Message.Chat,"Удаление отменено", cancellationToken:ct, replyMarkup: KeyboardHelper.GetDefaultKeyboard());
                }

                return ScenarioResult.Completed;
            default:
                throw new ArgumentOutOfRangeException($"Непредусмотренный к обработке шаг \"{context.CurrentStep}\"");
        }
    }
}