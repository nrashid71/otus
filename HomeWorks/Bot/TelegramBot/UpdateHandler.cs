using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Bot.Dto;

namespace Bot;

public delegate void MessageEventHandler(string message);
public class UpdateHandler : IUpdateHandler
{
    private readonly List<string> _registredUserCommands = new List<string>() {"/addtask","/show","/removetask","/completetask","/cancel","/exit","/start","/report","/find"};

    private readonly IToDoService _toDoService;

    private readonly IUserService _userService;

    private readonly IScenario[] _scenarios;

    private readonly IScenarioContextRepository _contextRepository;

    private readonly IToDoListService _toDoListService;

    private int _pageSize = 5;
    
    /// <summary>
    /// Левая граница диапазона значений для максимально количества задач.
    /// </summary>
    const int MinCountLimit = 0;

    /// <summary>
    /// Правая граница диапазона значений для максимально количества задач.
    /// </summary>
    const int MaxCountLimit = 100;

    /// <summary>
    /// Левая граница диапазона допустимой длины задач.
    /// </summary>
    const int MinLengthLimit = 1;

    /// <summary>
    /// Правая граница диапазона допустимой длины задач.
    /// </summary>
    const int MaxLengthLimit = 1000;

    public UpdateHandler(IToDoService toDoService,
                        IUserService userService,
                        IEnumerable<IScenario> scenarios,
                        IScenarioContextRepository contextRepository,
                        IToDoListService toDoListService)
    {
        _toDoService = toDoService;
        _userService = userService;
        _contextRepository = contextRepository;
        _scenarios = (IScenario[]) scenarios;
        _toDoListService = toDoListService;
    }

    IScenario GetScenario(ScenarioType scenario)
    {
        var result = _scenarios.FirstOrDefault(s => s.CanHandle(scenario));//(s => s.CanHandle(scenario));

        if (result == null)
        {
            throw new KeyNotFoundException($"Scenario {scenario} not found");
        }
        
        return result;
    }

    private async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
    {
        var scenario = GetScenario(context.CurrentScenario);

        var scenarioResult = await scenario.HandleMessageAsync(botClient, context, update, ct);

        var userId =  update.Message?.From?.Id ?? update.CallbackQuery?.From?.Id ?? 0;
            
        switch (scenarioResult)
        {
            case ScenarioResult.Completed :
                await _contextRepository.ResetContext(userId, ct);
                break;
            case ScenarioResult.Transition:
                await _contextRepository.SetContext(userId, context, ct);
                break;
        }
    }

    public event MessageEventHandler? UpdateStarted;
    public void OnHandleUpdateStarted(string message)
    {
        Console.WriteLine($"Началась обработка сообщения '{message}'");
    }
    public event MessageEventHandler? UpdateCompleted;
    public void OnHandleUpdateCompleted(string message)
    {
        Console.WriteLine($"Закончилась обработка сообщения '{message}'");
    }
    
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"HandleError: {exception})");
        return Task.CompletedTask;
    }
    private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var toDoUser = (await _userService.GetUser(update.CallbackQuery.From.Id, ct));
        if (toDoUser == null)
        {
            return;
        }
        
        ScenarioContext context = await _contextRepository.GetContext(update?.CallbackQuery?.From?.Id ?? 0, ct);
            
        if (context != null && !string.IsNullOrEmpty(context.CurrentStep))
        {
            await ProcessScenario(botClient, context, update, ct);
            
            return;
        }
        
        var callbackDto = CallbackDto.FromString(update.CallbackQuery.Data);
        ScenarioType  scenario = ScenarioType.None;
        switch (callbackDto.Action)
        {
            case "show" :
                await ShowTasksAsync(PagedListCallbackDto.FromString(update.CallbackQuery.Data), botClient, update, ct);
                return;
            case "showtask" :
                await ShowOneTaskAsync(ToDoItemCallbackDto.FromString(update.CallbackQuery.Data), botClient, update, ct);
                return;
            case "show_completed":
                var dto = PagedListCallbackDto.FromString(update.CallbackQuery.Data);
                dto.ToDoItemState = ToDoItemState.Completed;
                await ShowTasksAsync(dto, /*ToDoItemState.Completed,*/ botClient, update, ct);
                return;
            case "completetask" :
                _toDoService.MarkCompleted(ToDoItemCallbackDto.FromString(update.CallbackQuery.Data).ToDoItemId, ct);
                await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Задача выполнена", cancellationToken: ct);
                return;
            case "deletetask" :
                // _toDoService.Delete(ToDoItemCallbackDto.FromString(update.CallbackQuery.Data).ToDoItemId);                
                // await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Задача удалена", cancellationToken: ct);
                // return;
                scenario = ScenarioType.DeleteTask;
                break;
            case "addlist" :
                scenario = ScenarioType.AddList;
                break;
            case "deletelist" :
                scenario = ScenarioType.DeleteList;
                break;
            case "addtask" :
                return;
        }
        
        context = new ScenarioContext(scenario, toDoUser.TelegramUserId);

        await _contextRepository.SetContext(update.CallbackQuery.From.Id, context, ct);

        await ProcessScenario(botClient, context, update, ct);
        
    }
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await OnCallbackQuery(botClient, update, ct);
            return;
        }
        string botCommand;
        string InfoMessage = "Вам доступны команды: start, help, info, addtask, show, cancel, report, find, exit. При вводе команды указываейте вначале символ / (слеш).";
        try
        {
            var commands = new List<BotCommand>
            {
                new BotCommand {Command = "start", Description = "Старт бота"},
                new BotCommand {Command = "help", Description = "Подсказка по командам бота"},
                new BotCommand {Command = "info", Description = "Информация по версии и дате версии бота"},
                new BotCommand {Command = "addtask", Description = "Добавление новой задачи"},
                new BotCommand {Command = "show", Description = "Отображение списка задач"},
                new BotCommand {Command = "cancel", Description = "Отмена выполнения сценария"},
                new BotCommand {Command = "report", Description = "Статистика по задачам"},
                new BotCommand {Command = "find", Description = "Поиск задачи"},
                new BotCommand {Command = "exit", Description = "Завершение работы с ботом"}
            };
            var context = await _contextRepository.GetContext(update?.Message?.From?.Id ?? 0, ct);
            
            if (update.Message.Text  == "/cancel")
            {
                await _contextRepository.ResetContext(update?.Message?.From?.Id ?? 0, ct);
                await botClient.SendMessage(update.Message.Chat, "Текущий сценарий отменен", replyMarkup: KeyboardHelper.GetDefaultKeyboard(), cancellationToken: ct);
                return;
            }
            
            if (context != null && !string.IsNullOrEmpty(context.CurrentStep))
            {
                await ProcessScenario(botClient, context, update, ct);
                return;
            }
            
            await botClient.SetMyCommands(commands);
            botCommand = update.Message.Text;
            var toDoUser = (await _userService.GetUser(update.Message.From.Id, ct));
            ReplyKeyboardMarkup replyMarkup;
            if (toDoUser == null)
            {
                if (botCommand != "/start")
                {
                    replyMarkup = new ReplyKeyboardMarkup(
                        new[] { new KeyboardButton[] { "/start" } });
                    await botClient.SendMessage(
                        update.Message.Chat,
                        "Зарегистрируйтесь, выполнив команду start",
                        cancellationToken: ct,
                        replyMarkup: replyMarkup);
                }
                else
                {
                    replyMarkup = new ReplyKeyboardMarkup();
                }
            }
            else
            {
                replyMarkup = KeyboardHelper.GetDefaultKeyboard();
            }
            replyMarkup.ResizeKeyboard = true;
            UpdateStarted.Invoke(botCommand);
            switch (botCommand)
            {
                case "/help":
                    await HelpAsync(botClient, update, ct, replyMarkup);
                    break;
                case "/info":
                    await InfoAsync(botClient, update, ct, replyMarkup);
                    break;
                case "/start":
                    await Start(botClient, update, ct);
                    break;
                default:
                    var idx = botCommand.IndexOf(" ");
                    if (_registredUserCommands.Contains(botCommand.Substring(0, idx == -1 ? botCommand.Length : idx)
                            .Trim()))
                    {
                        if (toDoUser != null)
                        {
                            switch (botCommand)
                            {
                                case "/exit":
//                                    Environment.Exit(0);
                                    break;
                                case "/addtask":
                                    await AddTaskAsync(botClient, update, ct, replyMarkup);
                                    break;
                                case "/show":
                                    await ShowAsync(botClient, update, ct);
                                    break;
                                case "/report":
                                    await ReportAsync(botClient, update, ct, replyMarkup);
                                    break;
                                case "/cancel":
                                    await CancelAsync(botClient, update, ct);
                                    break;
                                case string bc when bc.StartsWith("/find "):
                                    await FindAsync(botClient, update, botCommand.Substring("/find ".Length), ct, replyMarkup);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        await NonCommandAsync(botClient, update, botCommand, InfoMessage, ct, replyMarkup);
                    }

                    break;
            }
            UpdateCompleted.Invoke(botCommand);
        }
        catch (DuplicateTaskException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken:ct);
        }
        catch (TaskCountLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken:ct);
        }
        catch (TaskLengthLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken:ct);
        }
        catch (ArgumentException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken:ct);
        }
        catch (Exception ex)
        {
//            await botClient.SendMessage(update.Message.Chat, ex.Message + ex.StackTrace, cancellationToken:ct);
            Console.WriteLine(ex.Message + "\n" +ex.StackTrace);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Метод для начала сеанса взаимодействия бота с пользователм
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns>Имя пользователя</returns>
     async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var from = update?.Message?.From;
        var toDoUser = await _userService.RegisterUser(from?.Id ?? 0, from?.Username, cancellationToken);
        if (toDoUser != null)
        {
            await botClient.SendMessage(
                update.Message.Chat,
                "Для отображения информации по задачам можно воспользоваться кнопками",
                cancellationToken: cancellationToken,
                replyMarkup: KeyboardHelper.GetDefaultKeyboard());
        }
    }
   
    /// <summary>
    /// Вывод информации о том, что делает бот - команда /help.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task HelpAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        string helpMessage = @"Бот предназначен для управления списком задач (to do list)
Список допустимых команд:
 /start         - старт работы бота
 /help          - подсказка по командам бота (текущий текст)
 /info          - информация по версии и дате версии бота
 /addtask       - добавление новой задачи
 /show     - отображение списка задач
 /removetask    - удаление задачи
 /completetask  - завершение задачи
 /showalltasks  - отображение списка задач со статусами
 /cancel        - отмена выполнения сценария
 /report        - статистика по задачам
 /find          - поиск задачи
 /exit          - завершение работы с ботом";
        await botClient.SendMessage(update.Message.Chat, await ReplayAsync(update, helpMessage, ct), cancellationToken:ct, replyMarkup: replyMarkup);
    }
    
    /// <summary>
    /// Обработка введенной строк пользователем, которая не была распознана как команда.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="str">Введенная строка.</param>
    /// <param name="infoMessage">Сообщение для пользователя.</param>
    async Task NonCommandAsync(ITelegramBotClient botClient, Update update, string str, string infoMessage, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        if (!string.IsNullOrEmpty(str))
        {
            await botClient.SendMessage(update.Message.Chat,await ReplayAsync(update,$"Команда {str} не предусмотрена к обработке.\n" + infoMessage, ct), cancellationToken:ct, replyMarkup: replyMarkup);
        }
    }

    /// <summary>
    /// Вывод информации о программе.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task InfoAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        await botClient.SendMessage(update.Message.Chat,await ReplayAsync(update,"Версия программы 0.1.0-alpha. Дата создания 22.02.2025.", ct), cancellationToken:ct, replyMarkup: replyMarkup);
    }
    
    /// <summary>
    ///  Формируется строка сообщения, при необходимости с обращением к пользователю в зависимости указано или нет имя пользователя. Если имя пользователя не задано,
    ///  тогда возвращаетя значение параметра message как есть. Иначе, возвращается строка с корректным обращением к пользователю
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <returns></returns>
    async Task<string> ReplayAsync (Update update, string message, CancellationToken ct)
    {
        var toDoUser = await _userService.GetUser(update.Message.From.Id, ct);
        if (toDoUser == null || string.IsNullOrEmpty(toDoUser.TelegramUserName))
        {
            return message;
        }
        return $"{toDoUser.TelegramUserName}, " + message?.First().ToString().ToLower() + message?.Substring(1);
    }
    
    /// <summary>
    /// Добавление задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task AddTaskAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        var toDoUser = await _userService.GetUser(update.Message.From.Id, ct);
        
        ScenarioContext context = new ScenarioContext(ScenarioType.AddTask, toDoUser.TelegramUserId);

        await _contextRepository.SetContext(update.Message.From.Id, context, ct);

        await ProcessScenario(botClient, context, update, ct);
        
    }

    /// <summary>
    /// Отображение списка задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task ShowAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var toDoUser = await _userService.GetUser(update.Message.From.Id,ct);
        var userId = toDoUser?.UserId ?? Guid.Empty;
        
        List<InlineKeyboardButton[]> inlineKeyboardButtonsList = new List<InlineKeyboardButton[]>()
        {
            new []{InlineKeyboardButton.WithCallbackData("📌Без списка", new ToDoListCallbackDto("show").ToString())}
        };

        var r = _toDoListService.GetUserLists(userId, ct).Result.Select(
            l => new[] { InlineKeyboardButton.WithCallbackData(l.Name, new ToDoListCallbackDto("show", l.Id).ToString()) }
        );
        
        inlineKeyboardButtonsList.AddRange(r);

        inlineKeyboardButtonsList.Add(new []
        {
            InlineKeyboardButton.WithCallbackData("\ud83c\udd95Добавить", "addlist"),
            InlineKeyboardButton.WithCallbackData("\u274cУдалить", "deletelist"),
        });
        var inlineKeyboard = new InlineKeyboardMarkup(inlineKeyboardButtonsList);
        await botClient.SendMessage(update.Message.Chat.Id,
                                "Выберите список",
                                    cancellationToken:ct,
                                    //parseMode:ParseMode.MarkdownV2,
                                    replyMarkup: inlineKeyboard);
    }
    async Task ShowTasksAsync(PagedListCallbackDto dtoList, /*ToDoItemState toDoItemState,*/ ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var toDoUser = await _userService.GetUser(update.CallbackQuery.From.Id, ct);
        var userId = toDoUser?.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            throw new Exception("Нельзя отобразить список задач, так как пользователь не зарегистрирован в боте.");
        }

        var callbackData = _toDoService.GetByUserIdAndList(userId, dtoList.ToDoListId, ct).Result
            .Where(t => t.State == dtoList.ToDoItemState)
            .Select(i => new KeyValuePair<string,string>(i.Name, "showtask|" + i.Id))
            .ToList()
            .AsReadOnly();

        await botClient.EditMessageText(
            chatId: update.CallbackQuery.Message.Chat.Id,
            messageId: update.CallbackQuery.Message.Id,
            text: ((callbackData.Count == 0) ? "Задач нет":((dtoList.ToDoItemState == ToDoItemState.Active)?"Активные задачи":"Выполненные задачи")),
            replyMarkup: BuildPagedButtons(callbackData, dtoList),
            cancellationToken : ct
            );
    }
    private InlineKeyboardMarkup BuildPagedButtons(IReadOnlyList<KeyValuePair<string, string>> callbackData, PagedListCallbackDto listDto)
    {
        List<InlineKeyboardButton[]> inlineKeyboardButtonsList = new List<InlineKeyboardButton[]>();
        
        inlineKeyboardButtonsList.AddRange(
            callbackData
                        .Select(t => new[] { InlineKeyboardButton.WithCallbackData(t.Key, t.Value) })
                        .GetBatchByNumber(_pageSize, listDto.Page)
                );
        List<InlineKeyboardButton>  inlineKeyboardButtons = new List<InlineKeyboardButton>();
        
        if (listDto.Page > 0)
        {
            inlineKeyboardButtons.Add(InlineKeyboardButton.WithCallbackData("⬅️", (new PagedListCallbackDto("show", listDto.ToDoListId, listDto.Page - 1, listDto.ToDoItemState)).ToString()));
        }
        
        if (listDto.Page < (double)callbackData.Count / _pageSize - 1)
        {
            inlineKeyboardButtons.Add(InlineKeyboardButton.WithCallbackData("\u27a1\ufe0f", (new PagedListCallbackDto("show", listDto.ToDoListId, listDto.Page + 1, listDto.ToDoItemState)).ToString()));
        }

        inlineKeyboardButtonsList.Add(inlineKeyboardButtons.ToArray());
        
        inlineKeyboardButtonsList.Add(new[]{InlineKeyboardButton.WithCallbackData("\u2611\ufe0fПосмотреть выполненные", (new PagedListCallbackDto("show_completed", listDto.ToDoListId, 0, listDto.ToDoItemState)).ToString())});
            
        return new InlineKeyboardMarkup(inlineKeyboardButtonsList);
    }


    async Task ShowOneTaskAsync(ToDoItemCallbackDto toDoItemCallbackDto, ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var toDoUser = await _userService.GetUser(update.CallbackQuery.From.Id, ct);
        var userId = toDoUser?.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            throw new Exception("Нельзя отобразить список задач, так как пользователь не зарегистрирован в боте.");
        }
        
        List<InlineKeyboardButton[]> inlineKeyboardButtonsList = new List<InlineKeyboardButton[]>();

        inlineKeyboardButtonsList.Add(new []
        {
            InlineKeyboardButton.WithCallbackData("\u2705Выполнить", "completetask|" + toDoItemCallbackDto.ToDoItemId),
            InlineKeyboardButton.WithCallbackData("\u274cУдалить", "deletetask|" + toDoItemCallbackDto.ToDoItemId),
        });
            
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(inlineKeyboardButtonsList);

        var toDoItem = _toDoService.Get(toDoItemCallbackDto.ToDoItemId, ct).Result;
        await botClient.EditMessageText(
            chatId: update.CallbackQuery.Message.Chat.Id,
            messageId: update.CallbackQuery.Message.Id,
            text: $"{toDoItem.Name}\nСрок выполнения: {toDoItem.Deadline}\nВремя создания: {toDoItem.CreatedAt}" +
                  ((toDoItem.State== ToDoItemState.Completed )? $"\nВремя выполнения: {toDoItem.StateChangedAt}":""),
            replyMarkup: inlineKeyboard,
            cancellationToken : ct
        );
    }

    /// <summary>
    /// Выводит текст с запросом на ввод допустимого количества задач. Если введенное значение не входит в указанный диапазон значений, то генерируется исключение
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    async Task<int> GetTasksLimitAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимое количество задач ({MinCountLimit}-{MaxCountLimit}): ", cancellationToken:ct, replyMarkup: replyMarkup);
        string tasksLimitStr = Console.ReadLine() ?? "";
        return ParseAndValidateInt(tasksLimitStr, MinCountLimit, MaxCountLimit);
    }
    /// <summary>
    /// Выводит текст с запросом на ввод допустимого количества задач. Если введенное значение не входит в указанный диапазон значений, то генерируется исключение
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    async Task<int> GetTaskLengthLimitAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимую длину задачи ({MinLengthLimit}-{MaxLengthLimit} символов): ", cancellationToken:ct, replyMarkup: replyMarkup);
        string taskLengthLimitStr = Console.ReadLine() ?? "";
        return ParseAndValidateInt(taskLengthLimitStr, MinLengthLimit, MaxLengthLimit);
    }

    /// <summary>
    /// Приводит введенную пользователм строку к int и проверяет, что оно находится в диапазоне min и max.
    /// В противном случае выбрасывать ArgumentException с сообщением.
    /// </summary>
    /// <param name="str">Введенная пользователем строка</param>
    /// <param name="min">Левая граница диапазона допустимых значений, для вводимого пользователем значения.</param>
    /// <param name="max">Правая граница диапазона допустимых значений, для вводимого пользователем значения.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    int ParseAndValidateInt(string? str, int min, int max)
    {
        ValidateString(str);

        if (!int.TryParse(str, out int tasksLimit) || tasksLimit < min || tasksLimit > max)
        {
            throw new ArgumentException($"Ожидалось значение от {min} до {max}, а было введено значение \"{str}\"");
        }
        return tasksLimit;
    }

    /// <summary>
    /// Проверка на "непустое" значение строки.
    /// </summary>
    /// <param name="str">Проверяемая строка</param>
    /// <exception cref="ArgumentException"></exception>
    void ValidateString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException($"Недопустимое значение для строки - она не должна быть пустой и не должна содержать только пробельные символы");
        }
    }
    
    /// <summary>
    /// Статистика по задачам
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task ReportAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        var ( total, completed, active, generatedAt)
                = (new ToDoReportService(_toDoService)).GetUserStats((await _userService.GetUser(update.Message.From.Id, ct)).UserId, ct);
        
        await botClient.SendMessage(update.Message.Chat,$"Статистика по задачам на {generatedAt}." +
                                                  $" Всего: {total};" +
                                                  $" Завершенных: {completed}" +
                                                  $" Активных: {active};", cancellationToken:ct, replyMarkup: replyMarkup);
    }

    /// <summary>
    /// Отмена выполнения сценария
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task CancelAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var context = await _contextRepository.GetContext(update?.Message?.From?.Id ?? 0, ct);
        
        if (context != null && !string.IsNullOrEmpty(context.CurrentStep))
        {
            await _contextRepository.ResetContext(update?.Message?.From?.Id ?? 0, ct);

            await botClient.SendMessage(
                update.Message.Chat,
                "Отмена выполнения сценария",
                cancellationToken: ct,
                replyMarkup: KeyboardHelper.GetDefaultKeyboard());
        }
    }
    
    /// <summary>
    /// Поиск задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task FindAsync(ITelegramBotClient botClient, Update update, string namePrefix, CancellationToken ct, ReplyKeyboardMarkup replyMarkup)
    {
        foreach (var task in await _toDoService.Find((await _userService.GetUser(update.Message.From.Id, ct)), namePrefix, ct))
        {
            await botClient.SendMessage(update.Message.Chat,$"{task.Name} - {task.CreatedAt} - {task.Id}", cancellationToken:ct, replyMarkup: replyMarkup);
        }
    }
}