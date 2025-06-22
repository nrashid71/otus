using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Bot.Dto;

namespace Bot;

public class DeleteTaskScenario: IScenario
{
    private readonly IToDoService _toDoService;

    public DeleteTaskScenario(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }
    public bool CanHandle(ScenarioType scenario)
    {
        return scenario == ScenarioType.DeleteTask;
    }
    public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
    {
        switch (context.CurrentStep)
        {
            case null:
                var toDoItemCallbackDto = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                var toDoItem = _toDoService.Get(toDoItemCallbackDto.ToDoItemId, ct).Result;
                context.Data.Add("ToDoItem", toDoItem.Id);
                context.CurrentStep = "Delete";
                await bot.SendMessage(update.CallbackQuery.Message.Chat.Id,
                    $"Подтверждаете удаление задачи {toDoItem?.Name}",
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
                     var toDoItemId = (Guid)context.Data["ToDoItem"];
                     await _toDoService.Delete(toDoItemId);
                     await bot.SendMessage(update.CallbackQuery.Message.Chat,"Задача удалена", cancellationToken:ct, replyMarkup: KeyboardHelper.GetDefaultKeyboard());
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