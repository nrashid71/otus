using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public delegate void MessageEventHandler(string message);
public class UpdateHandler : IUpdateHandler
{
    private List<string> _registredUserCommands = new List<string>() {"/addtask","/showtask","/removetask","/completetask","/showalltasks","/exit","/start","/report","/find"};

    private IToDoService ToDoService { get; }
    
    private IUserService UserService { get; }

    /// <summary>
    /// Максимальное количество задач, указанное пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    int _taskCountLimit = -1;
    /// <summary>
    /// Максимальная длина задачи, указанная пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    int _taskLengthLimit = -1;

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
    const int MaxLengthLimit = 100;

    public UpdateHandler(IToDoService toDoService, IUserService userService)
    {
        ToDoService = toDoService;
        UserService = userService;
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
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        string botCommand;
        string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, completetask, showalltasks, report, find, exit. При вводе команды указываейте вначале символ / (слеш).";
        var toDoUser = UserService.GetUser(update.Message.From.Id);
        try
        {
            await botClient.SendMessage(update.Message.Chat, "Введите команду:", ct);
            botCommand = Console.ReadLine() ?? "";
            UpdateStarted.Invoke(botCommand);
            switch (botCommand)
            {
                case "/help":
                    await HelpAsync(botClient, update, ct);
                    break;
                case "/info":
                    await InfoAsync(botClient, update, ct);
                    break;
                case "/start":
                    Start(botClient, update);
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
                                    Environment.Exit(0);
                                    break;
                                case string bc when bc.StartsWith("/addtask "):
                                    await AddTaskAsync(botClient, update, botCommand.Substring("/addtask ".Length), ct);
                                    break;
                                case "/showtask":
                                    await ShowTasksAsync(botClient, update, ct);
                                    break;
                                case string bc when bc.StartsWith("/removetask "):
                                    await RemoveTaskAsync(botClient, update,
                                        botCommand.Substring("/removetask ".Length), ct);
                                    break;
                                case string bc when bc.StartsWith("/completetask "):
                                    CompleteTask(botCommand.Substring("/completetask ".Length));
                                    break;
                                case "/showalltasks":
                                    await ShowAllTasksAsync(botClient, update, ct);
                                    break;
                                case "/report":
                                    await ReportAsync(botClient, update, ct);
                                    break;
                                case string bc when bc.StartsWith("/find "):
                                    await FindAsync(botClient, update, botCommand.Substring("/find ".Length), ct);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        await NonCommandAsync(botClient, update, botCommand, InfoMessage, ct);
                    }

                    break;
            }
            UpdateCompleted.Invoke(botCommand);
        }
        catch (DuplicateTaskException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (TaskCountLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (TaskLengthLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (ArgumentException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
    }

    /// <summary>
    /// Метод для начала сеанса взаимодействия бота с пользователм
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns>Имя пользователя</returns>
    void Start(ITelegramBotClient botClient, Update update)
    {
        var from = update?.Message?.From;
        var toDoUser = UserService.RegisterUser(from?.Id ?? 0, from?.Username);
    }
    
    /// <summary>
    /// Вывод информации о том, что делает бот - команда /help.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task HelpAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        string helpMessage = @"Бот предоставляет краткую информацию по ключевым словам C# с небольшими примерами. Примеры ключевых слов abstract, event, namespace.
Список допустимых команд:
 /start         - старт работы бота
 /help          - подсказка по командам бота (текущий текст)
 /info          - информация по версии и дате версии бота
 /addtask       - добавление новой задачи
 /showtasks     - отображение списка задач
 /removetask    - удаление задачи
 /completetask  - завершение задачи
 /showalltasks  - отображение списка задач со статусами
 /report        - статистика по задачам
 /find          - поиск задачи
 /exit          - завершение работы с ботом";
        await botClient.SendMessage(update.Message.Chat, Replay(update, helpMessage), ct);
    }
    
    /// <summary>
    /// Обработка введенной строк пользователем, которая не была распознана как команда.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="str">Введенная строка.</param>
    /// <param name="infoMessage">Сообщение для пользователя.</param>
    async Task NonCommandAsync(ITelegramBotClient botClient, Update update, string str, string infoMessage, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(str))
        {
            await botClient.SendMessage(update.Message.Chat,Replay(update,$"Команда {str} не предусмотрена к обработке."), ct);
            await botClient.SendMessage(update.Message.Chat,infoMessage, ct);
        }
    }

    /// <summary>
    /// Вывод информации о программе.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task InfoAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        await botClient.SendMessage(update.Message.Chat,Replay(update,"Версия программы 0.1.0-alpha. Дата создания 22.02.2025."), ct);
    }
    
    /// <summary>
    ///  Формируется строка сообщения, при необходимости с обращением к пользователю в зависимости указано или нет имя пользователя. Если имя пользователя не задано,
    ///  тогда возвращаетя значение параметра message как есть. Иначе, возвращается строка с корректным обращением к пользователю
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <returns></returns>
    string Replay (Update update, string message)
    {
        var toDoUser = UserService.GetUser(update.Message.From.Id);
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
    async Task AddTaskAsync(ITelegramBotClient botClient, Update update, string description, CancellationToken ct)
    {
        await CheckTasksAsync(botClient, update, ct);
        var userId = UserService.GetUser(update.Message.From.Id).UserId;
        if (ToDoService.GetAllByUserId(userId).Count >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length > _taskLengthLimit)
            {
                throw new TaskLengthLimitException(description.Length, _taskLengthLimit);
            }

            if (ToDoService.GetAllByUserId(userId).Any(t => t.Name == description))
            {
                throw new DuplicateTaskException(description);
            }
            
            ToDoService.Add(UserService.GetUser(update.Message.From.Id), description);
            
            await botClient.SendMessage(update.Message.Chat,"Задача добавлена.", ct);
        }
    }

    /// <summary>
    /// Отображение списка задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task ShowTasksAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var userId = UserService.GetUser(update.Message.From.Id).UserId;
        if (ToDoService.GetAllByUserId(userId).Count  == 0)
        {
            await botClient.SendMessage(update.Message.Chat,"Список задач пуст.", ct);
        }
        else
        {
            foreach (var task in ToDoService.GetActiveByUserId(userId))
            {
                await botClient.SendMessage(update.Message.Chat,$"{task.Name} - {task.CreatedAt} - {task.Id}", ct);
            }
        }
    }

    /// <summary>
    /// Удаление задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task RemoveTaskAsync(ITelegramBotClient botClient, Update update, string stringGuid, CancellationToken ct)
    {
        await ShowTasksAsync(botClient, update, ct);
        
        var userId = UserService.GetUser(update.Message.From.Id).UserId;
        
        if (ToDoService.GetAllByUserId(userId).Count  == 0) {
            await botClient.SendMessage(update.Message.Chat,"Список задач пуст, удалять нечего.", ct);
            return;
        }

        await botClient.SendMessage(update.Message.Chat,"Укажите номер задачи, которую Вы хотите удалить:", ct);
                
        if (Guid.TryParse(stringGuid, out var guid))
        {
            ToDoService.Delete(guid);
            await botClient.SendMessage(update.Message.Chat,$"Задача с id \"{stringGuid}\" удалена.", ct);
        }
        else
        {
            await botClient.SendMessage(update.Message.Chat, $"Задачи с id \"{stringGuid}\" нет.", ct);
        }
    }

    /// <summary>
    /// Проверка списка задач на количество и длину задания
    /// </summary>
    private async Task CheckTasksAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (_taskCountLimit == -1)
        {
            _taskCountLimit = await GetTasksLimitAsync(botClient, update, ct);
        }
        if (_taskLengthLimit == -1)
        {
            _taskLengthLimit = await GetTaskLengthLimitAsync(botClient, update, ct);
        }
    }

    /// <summary>
    /// Выводит текст с запросом на ввод допустимого количества задач. Если введенное значение не входит в указанный диапазон значений, то генерируется исключение
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    async Task<int> GetTasksLimitAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        await botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимое количество задач ({MinCountLimit}-{MaxCountLimit}): ", ct);
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
    async Task<int> GetTaskLengthLimitAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        await botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимую длину задачи ({MinLengthLimit}-{MaxLengthLimit} символов): ", ct);
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
    /// Завершение задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    void CompleteTask(string stringGuid)
    {
        if (Guid.TryParse(stringGuid, out var guid))
        {
            ToDoService.MarkCompleted(guid);
        }
        else
        {
            throw new ArgumentException($"Недопустимое значение для строкового представления Guid - должно состоять 32 цифр, разделенные дефисом");
        }
    }

    /// <summary>
    /// Отображение задач с их статусом
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task ShowAllTasksAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var userId = UserService.GetUser(update.Message.From.Id).UserId;
        
        if (ToDoService.GetAllByUserId(userId).Count  == 0)
        {
            await botClient.SendMessage(update.Message.Chat,Replay(update,"Список задач пуст."), ct);
        }
        else
        {
            foreach (var task in ToDoService.GetAllByUserId(userId))
            {
                await botClient.SendMessage(update.Message.Chat,$"({Enum.GetName(task.State)}) {task.Name} - {task.CreatedAt} - {task.Id}", ct);
            }
        }
    }
    /// <summary>
    /// Статистика по задачам
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task ReportAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var ( total, completed, active, generatedAt)
                = (new ToDoReportService(ToDoService)).GetUserStats(UserService.GetUser(update.Message.From.Id).UserId);
        
        await botClient.SendMessage(update.Message.Chat,$"Статистика по задачам на {generatedAt}." +
                                                  $" Всего: {total};" +
                                                  $" Завершенных: {completed}" +
                                                  $" Активных: {active};", ct);
    }

    /// <summary>
    /// Поиск задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    async Task FindAsync(ITelegramBotClient botClient, Update update, string namePrefix, CancellationToken ct)
    {
        foreach (var task in ToDoService.Find(UserService.GetUser(update.Message.From.Id), namePrefix))
        {
            await botClient.SendMessage(update.Message.Chat,$"{task.Name} - {task.CreatedAt} - {task.Id}", ct);
        }
    }
}