using System.Collections.Immutable;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;
public class ToDoService : IToDoService
{
    /// <summary>
    /// Список задач
    /// </summary>
    InMemoryToDoRepository _botTasks = new InMemoryToDoRepository();
    
    InMemoryToDoRepository InMemoryToDoRepository
    {
        get => _botTasks;
    }
    /// <summary>
    /// Сервис по упралению пользователями
    /// </summary>
    UserService _userService = new UserService();
    
    public UserService UserService {get => _userService;}
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

//    public ToDoUser? toDoUser { get; set; }

    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return InMemoryToDoRepository.GetAllByUserId(userId);
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return InMemoryToDoRepository.GetActiveByUserId(userId);
    }

    public ToDoItem Add(ToDoUser user, string name)
    {
        ToDoItem toDoItem = new ToDoItem(name, user);
        
        InMemoryToDoRepository.Add(toDoItem);
        
        return toDoItem;
    }

    public void MarkCompleted(Guid id)
    {
        var task = InMemoryToDoRepository.GetByGuid(id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
        }
    }

    public void Delete(Guid id)
    {
        InMemoryToDoRepository.Delete(id);
    }

    /// <summary>
    /// Метод для начала сеанса взаимодействия бота с пользователм
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns>Имя пользователя</returns>
    public void Start(ITelegramBotClient botClient, Update update)
    {
        var from = update?.Message?.From;
        var toDoUser = _userService.RegisterUser(from?.Id ?? 0, from?.Username);
    }
    
    /// <summary>
    /// Вывод информации о том, что делает бот - команда /help.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void Help(ITelegramBotClient botClient, Update update)
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
 /exit          - завершение работы с ботом";
        botClient.SendMessage(update.Message.Chat, Replay(update, helpMessage));
    }
    
    /// <summary>
    /// Обработка введенной строк пользователем, которая не была распознана как команда.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="str">Введенная строка.</param>
    /// <param name="infoMessage">Сообщение для пользователя.</param>
    public void NonCommand(ITelegramBotClient botClient, Update update, string str, string infoMessage)
    {
        if (!string.IsNullOrEmpty(str))
        {
            botClient.SendMessage(update.Message.Chat,Replay(update,$"Команда {str} не предусмотрена к обработке."));
            botClient.SendMessage(update.Message.Chat,infoMessage);
        }
    }

    /// <summary>
    /// Вывод информации о программе.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void Info(ITelegramBotClient botClient, Update update)
    {
        botClient.SendMessage(update.Message.Chat,Replay(update,"Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
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
    public void AddTask(ITelegramBotClient botClient, Update update, string description)
    {
        CheckTasks(botClient, update);
        var userId = _userService.GetUser(update.Message.From.Id).UserId;
        if (InMemoryToDoRepository.GetCount(userId) >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length > _taskLengthLimit)
            {
                throw new TaskLengthLimitException(description.Length, _taskLengthLimit);
            }

            if (InMemoryToDoRepository.ExistsByName(userId, description))
            {
                throw new DuplicateTaskException(description);
            }
            
            InMemoryToDoRepository.Add(new ToDoItem(description, UserService.GetUser(update.Message.From.Id)));
            
            botClient.SendMessage(update.Message.Chat,"Задача добавлена.");
        }
    }

    /// <summary>
    /// Отображение списка задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void ShowTasks(ITelegramBotClient botClient, Update update)
    {
        var userId = _userService.GetUser(update.Message.From.Id).UserId;
        if (InMemoryToDoRepository.GetCount(userId) == 0)
        {
            botClient.SendMessage(update.Message.Chat,"Список задач пуст.");
        }
        else
        {
            foreach (var task in InMemoryToDoRepository.GetActiveByUserId(userId))
            {
                botClient.SendMessage(update.Message.Chat,$"{task.Name} - {task.CreatedAt} - {task.Id}");
            }
        }
    }

    /// <summary>
    /// Удаление задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void RemoveTask(ITelegramBotClient botClient, Update update, string stringGuid)
    {
        ShowTasks(botClient, update);
        
        var userId = _userService.GetUser(update.Message.From.Id).UserId;
        
        if (InMemoryToDoRepository.GetCount(userId) == 0) {
            botClient.SendMessage(update.Message.Chat,"Список задач пуст, удалять нечего.");
            return;
        }

        botClient.SendMessage(update.Message.Chat,"Укажите номер задачи, которую Вы хотите удалить:");
                
        if (Guid.TryParse(stringGuid, out var guid))
        {
            InMemoryToDoRepository.Delete(guid);
            botClient.SendMessage(update.Message.Chat,$"Задача с id \"{stringGuid}\" удалена.");
        }
        else
        {
            botClient.SendMessage(update.Message.Chat, $"Задачи с id \"{stringGuid}\" нет.");
        }
    }

    /// <summary>
    /// Проверка списка задач на количество и длину задания
    /// </summary>
    private void CheckTasks(ITelegramBotClient botClient, Update update)
    {
        if (_taskCountLimit == -1)
        {
            _taskCountLimit = GetTasksLimit(botClient, update);
        }
        if (_taskLengthLimit == -1)
        {
            _taskLengthLimit = GetTaskLengthLimit(botClient, update);
        }
    }

    /// <summary>
    /// Выводит текст с запросом на ввод допустимого количества задач. Если введенное значение не входит в указанный диапазон значений, то генерируется исключение
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    int GetTasksLimit(ITelegramBotClient botClient, Update update)
    {
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимое количество задач ({MinCountLimit}-{MaxCountLimit}): ");
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
    int GetTaskLengthLimit(ITelegramBotClient botClient, Update update)
    {
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимую длину задачи ({MinLengthLimit}-{MaxLengthLimit} символов): ");
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
    public void CompleteTask(string stringGuid)
    {
        if (Guid.TryParse(stringGuid, out var guid))
        {
            MarkCompleted(guid);
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
    public void ShowAllTasks(ITelegramBotClient botClient, Update update)
    {
        var userId = _userService.GetUser(update.Message.From.Id).UserId;
        
        if (InMemoryToDoRepository.GetCount(userId) == 0)
        {
            botClient.SendMessage(update.Message.Chat,Replay(update,"Список задач пуст."));
        }
        else
        {
            foreach (var task in InMemoryToDoRepository.GetAllByUserId(userId))
            {
                botClient.SendMessage(update.Message.Chat,$"({Enum.GetName(task.State)}) {task.Name} - {task.CreatedAt} - {task.Id}");
            }
        }
    }
    /// <summary>
    /// Статистика по задачам
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void Report(ITelegramBotClient botClient, Update update)
    {
        var ( total, completed, active, generatedAt)
                = (new ToDoReportService(InMemoryToDoRepository)).GetUserStats(_userService.GetUser(update.Message.From.Id).UserId);
        
        botClient.SendMessage(update.Message.Chat,$"Статистика по задачам на {generatedAt}." +
                                                  $" Всего: {total};" +
                                                  $" Завершенных: {completed}" +
                                                  $" Активных: {active};");
    }

    public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
    {
        return InMemoryToDoRepository.Find(user.UserId, (t) => t.Name.StartsWith(namePrefix)).ToList()
            .AsReadOnly();
    }
    
    /// <summary>
    /// Поиск задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    public void Find(ITelegramBotClient botClient, Update update, string namePrefix)
    {
        foreach (var task in Find(_userService.GetUser(update.Message.From.Id), namePrefix))
        {
            botClient.SendMessage(update.Message.Chat,$"{task.Name} - {task.CreatedAt} - {task.Id}");
        }
    }
    
}


