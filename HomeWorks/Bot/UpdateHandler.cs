using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public class UpdateHandler : IUpdateHandler
{
    /// <summary>
    /// Список задач
    /// </summary>
    List<string> BotTasks = new List<string>();
        
    /// <summary>
    /// Максимальное количество задач, указанное пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    int taskCountLimit = -1;

    /// <summary>
    /// Максимальная длина задачи, указанная пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
    /// </summary>
    int taskLengthLimit = -1;

    /// <summary>
    /// Левая граница диапазона значений для максимально количества задач.
    /// </summary>
    const int minCountLimit = 0;

    /// <summary>
    /// Правая граница диапазона значений для максимально количества задач.
    /// </summary>
    const int maxCountLimit = 100;

    /// <summary>
    /// Левая граница диапазона допустимой длины задач.
    /// </summary>
    const int minLengthLimit = 1;

    /// <summary>
    /// Правая граница диапазона допустимой длины задач.
    /// </summary>
    const int maxLengthLimit = 100;
    
    public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
            bool run = true;
            string BotCommand;
            string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, exit. При вводе команды указываейте вначале символ / (слеш).";
            string UserName = "";
            botClient.SendMessage(update.Message.Chat,"Здравствуйте!");
            botClient.SendMessage(update.Message.Chat,InfoMessage);
            while (run)
            {
                try
                {
                    if (taskCountLimit == -1)
                    {
                        taskCountLimit = GetTasksLimit(botClient, update);
                    }
                    if (taskLengthLimit == -1)
                    {
                        taskLengthLimit = GetTaskLengthLimit(botClient, update);
                    }
                    botClient.SendMessage(update.Message.Chat,"Введите команду:");
                    BotCommand = Console.ReadLine() ?? "";
                    switch (BotCommand)
                    {
                        case "/start":
                            UserName = Start(botClient, update);
                            break;
                        case "/help":
                            Help(botClient, update, UserName);
                            break;
                        case "/info":
                            Info(botClient, update, UserName);
                            break;
                        case "/exit":
                            run = false;
                            break;
                        case "/addtask":
                            AddTask(botClient, update);
                            break;
                        case "/showtask":
                            ShowTasks(botClient, update);
                            break;
                        case "/removetask":
                            RemoveTask(botClient, update);
                            break;
                        default:
                            NonCommand(botClient, update, BotCommand, UserName, InfoMessage);
                            break;
                    }
                }
                catch (DuplicateTaskException ex)
                {
                    botClient.SendMessage(update.Message.Chat,ex.Message);
                }
                catch (TaskCountLimitException ex)
                {
                    botClient.SendMessage(update.Message.Chat,ex.Message);
                }
                catch (TaskLengthLimitException ex)
                {
                    botClient.SendMessage(update.Message.Chat,ex.Message);
                }
                catch (ArgumentException ex)
                {
                    botClient.SendMessage(update.Message.Chat,ex.Message);
                }
            }
    }

    /// <summary>
    /// Метод для начала сеанса взаимодействия бота с пользователм
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <returns>Имя пользователя</returns>
    string Start(ITelegramBotClient botClient, Update update)
    {
        botClient.SendMessage(update.Message.Chat,"Введите Ваше имя:");
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Обработка введенной строк пользователем, которая не была распознана как команда.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="str">Введенная строка.</param>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="infoMessage">Сообщение для пользователя.</param>
    void NonCommand(ITelegramBotClient botClient, Update update, string str, string userName, string infoMessage)
    {
        if (!string.IsNullOrEmpty(str))
        {
            botClient.SendMessage(update.Message.Chat,Replay(userName, $"Команда {str} не предусмотрена к обработке."));
            botClient.SendMessage(update.Message.Chat,infoMessage);
        }
    }

    /// <summary>
    /// Вывод информации о программе.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="userName">Имя пользователя.</param>
    void Info(ITelegramBotClient botClient, Update update, string userName)
    {
        botClient.SendMessage(update.Message.Chat,Replay(userName, "Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
    }

    /// <summary>
    ///  Формируется строка сообщения, при необходимости с обращением к пользователю в зависимости указано или нет имя пользователя. Если имя пользователя не задано,
    ///  тогда возвращаетя значение параметра message как есть. Иначе, возвращается строка с корректным обращением к пользователю
    /// </summary>
    /// <param name="UserName">Имя пользователя</param>
    /// <param name="message">Текст сообщения</param>
    /// <returns></returns>
    string Replay (string UserName, string message)
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return message;
        }
        return $"{UserName}, " + message?.First().ToString().ToLower() + message?.Substring(1);
    }

    /// <summary>
    /// Добавление задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    void AddTask(ITelegramBotClient botClient, Update update)
    {
        if (BotTasks.Count >= taskCountLimit)
        {
            throw new TaskCountLimitException((int)taskCountLimit);
        }
        botClient.SendMessage(update.Message.Chat,"Введите описание задачи:");
        var description = Console.ReadLine() ?? "";
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length > taskLengthLimit)
            {
                throw new TaskLengthLimitException(description.Length, taskLengthLimit);
            }
            if (BotTasks.Contains(description))
            {
                throw new DuplicateTaskException(description);
            }
            BotTasks.Add(description);
            botClient.SendMessage(update.Message.Chat,"Задача добавлена.");
        }
    }

    /// <summary>
    /// Отображение списка задач
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    void ShowTasks(ITelegramBotClient botClient, Update update)
    {
        if (BotTasks.Count == 0)
        {
            botClient.SendMessage(update.Message.Chat,"Список задач пуст.");
        }
        else
        {
            for (int i = 0; i < BotTasks.Count; i++) {
                botClient.SendMessage(update.Message.Chat,$"{i+1}.{BotTasks[i]}");
            }
        }
    }

    /// <summary>
    /// Удаление задачи
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    void RemoveTask(ITelegramBotClient botClient, Update update)
    {
        ShowTasks(botClient, update);
        
        if (BotTasks.Count == 0) {
            botClient.SendMessage(update.Message.Chat,"Нет задач к удалению.");
            return;
        }

        string TaskNumStr;

        string UnallowableNumMessage;

        botClient.SendMessage(update.Message.Chat,"Укажите номер задачи, которую Вы хотите удалить:");

        while (true)
        {
            TaskNumStr = Console.ReadLine() ?? "";
            UnallowableNumMessage = $"Недопустимое значение для номера задачи \"{TaskNumStr}\".\nВведите корректный номер задачи:";
            if (int.TryParse(TaskNumStr, out int TaskNum))
            {
                if (TaskNum < 1 || TaskNum > BotTasks.Count)
                {
                    botClient.SendMessage(update.Message.Chat, UnallowableNumMessage);
                }
                else
                {
                    BotTasks.RemoveAt(TaskNum - 1);
                    botClient.SendMessage(update.Message.Chat,$"Задача под номером {TaskNum} удалена.");
                    break; // Выходим из бесконечного цикла
                }
            }
            else
            {
                botClient.SendMessage(update.Message.Chat, UnallowableNumMessage);
            }
        }
    }
    /// <summary>
    /// Вывод информации о том, что делает бот - команда /help.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    void Help(ITelegramBotClient botClient, Update update, string UserName)
    {
        string HelpMessage = @"Бот предоставляет краткую информацию по ключевым словам C# с небольшими примерами. Примеры ключевых слов abstract, event, namespace.
Список допустимых команд:
 /start      - старт работы бота
 /help       - подсказка по командам бота (текущий текст)
 /info       - информация по версии и дате версии бота
 /addtask    - добавление новой задачи
 /showtasks  - отображение списка задач
 /removetask - удаление задачи
 /exit       - завершение работы с ботом";
        botClient.SendMessage(update.Message.Chat,Replay(UserName, HelpMessage));

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
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимое количество задач ({minCountLimit}-{maxCountLimit}): ");
        string TasksLimitStr = Console.ReadLine() ?? "";
        return ParseAndValidateInt(TasksLimitStr, minCountLimit, maxCountLimit);
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
        botClient.SendMessage(update.Message.Chat, $"Введите максимально допустимую длину задачи ({minLengthLimit}-{maxLengthLimit} символов): ");
        string TaskLengthLimitStr = Console.ReadLine() ?? "";
        return ParseAndValidateInt(TaskLengthLimitStr, minLengthLimit, maxLengthLimit);
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

        if (!int.TryParse(str, out int TasksLimit) || TasksLimit < min || TasksLimit > max)
        {
            throw new ArgumentException($"Ожидалось значение от {min} до {max}, а было введено значение \"{str}\"");
        }
        return TasksLimit;
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
}