using System.Collections.Immutable;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;
public class ToDoService : IToDoService
{
    /// <summary>
    /// Список задач
    /// </summary>
    List<ToDoItem> _botTasks = new List<ToDoItem>();
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

    public ToDoService(ToDoUser? toDoUser)
    {
        this.toDoUser = toDoUser;
    }

    public ToDoUser? toDoUser { get; set; }

    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return _botTasks.Where(t => t.ToDoUser.UserId == userId).ToList().AsReadOnly();
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return _botTasks.Where(t => t.ToDoUser.UserId == userId && t.State == ToDoItemState.Active).ToList().AsReadOnly();
    }

    public ToDoItem Add(ToDoUser user, string name)
    {
        ToDoItem toDoItem = new ToDoItem(name, user);
        
        _botTasks.Add(toDoItem);
        
        return toDoItem;
    }

    public void MarkCompleted(Guid id)
    {
        var task = _botTasks.FirstOrDefault(t => t.Id == id);

        if (task != null)
        {
            task.State = ToDoItemState.Completed;
        }
    }

    public void Delete(Guid id)
    {
        _botTasks.RemoveAll(t => t.Id == id);
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
        toDoUser = (new UserService()).RegisterUser(from?.Id ?? 0, from?.Username);
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
 /exit          - завершение работы с ботом";
        botClient.SendMessage(update.Message.Chat, Replay(helpMessage));
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
            botClient.SendMessage(update.Message.Chat,Replay($"Команда {str} не предусмотрена к обработке."));
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
        botClient.SendMessage(update.Message.Chat,Replay("Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
    }
    
    /// <summary>
    ///  Формируется строка сообщения, при необходимости с обращением к пользователю в зависимости указано или нет имя пользователя. Если имя пользователя не задано,
    ///  тогда возвращаетя значение параметра message как есть. Иначе, возвращается строка с корректным обращением к пользователю
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <returns></returns>
    string Replay (string message)
    {
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
        if (_botTasks.Count >= _taskCountLimit)
        {
            throw new TaskCountLimitException((int)_taskCountLimit);
        }
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length > _taskLengthLimit)
            {
                throw new TaskLengthLimitException(description.Length, _taskLengthLimit);
            }

            if (_botTasks.Any(t => t.Name == description))
            {
                throw new DuplicateTaskException(description);
            }
            
            _botTasks.Add(new ToDoItem(description, toDoUser));
            
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
        if (_botTasks.Count == 0)
        {
            botClient.SendMessage(update.Message.Chat,"Список задач пуст.");
        }
        else
        {
            foreach (var task in _botTasks.Where(t => t.State == ToDoItemState.Active))
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
    public void RemoveTask(ITelegramBotClient botClient, Update update, string taskNumStr)
    {
        ShowTasks(botClient, update);
        
        if (_botTasks.Count == 0) {
            botClient.SendMessage(update.Message.Chat,"Нет задач к удалению.");
            return;
        }

        string unallowableNumMessage = $"Недопустимое значение для номера задачи \"{taskNumStr}\".\nВведите корректный номер задачи:";

        botClient.SendMessage(update.Message.Chat,"Укажите номер задачи, которую Вы хотите удалить:");

        while (true)
        {
            if (int.TryParse(taskNumStr, out int TaskNum))
            {
                if (TaskNum < 1 || TaskNum > _botTasks.Count)
                {
                    botClient.SendMessage(update.Message.Chat, unallowableNumMessage);
                }
                else
                {
                    _botTasks.RemoveAt(TaskNum - 1);
                    botClient.SendMessage(update.Message.Chat,$"Задача под номером {TaskNum} удалена.");
                    break; // Выходим из бесконечного цикла
                }
            }
            else
            {
                botClient.SendMessage(update.Message.Chat, unallowableNumMessage);
            }
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
        if (_botTasks.Count == 0)
        {
            botClient.SendMessage(update.Message.Chat,Replay("Список задач пуст."));
        }
        else
        {
            foreach (var task in _botTasks)
            {
                botClient.SendMessage(update.Message.Chat,$"({Enum.GetName(task.State)}) {task.Name} - {task.CreatedAt} - {task.Id}");
            }
        }
    }
}


