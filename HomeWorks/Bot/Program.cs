using System.Diagnostics;
using System.Threading.Tasks;

namespace Bot
{
    internal class Program
    {
        /// <summary>
        /// Список задач
        /// </summary>
        static List<string> BotTasks = new List<string>();
        
        /// <summary>
        /// Максимальное количество задач, указанное пользователем. Значение -1 указывает на то, что атрибут не проинициализирован пользователем через запрос.
        /// </summary>
        static int taskCountLimit = -1;

        /// <summary>
        /// Левая граница диапазона значений для максимально количества задач.
        /// </summary>
        const int minCountLimit = 0;

        /// <summary>
        /// Правая граница диапазона значений для максимально количества задач.
        /// </summary>
        const int maxCountLimit = 100;

        static void Main(string[] args)
        {
            bool run = true;
            string BotCommand;
            string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, exit. При вводе команды указываейте вначале символ / (слеш).";
            string UserName = "";
            Console.WriteLine("Здравствуйте!");
            Console.WriteLine(InfoMessage);
            while (run)
            {
                try
                {
                    if (taskCountLimit == -1)
                    {
                        taskCountLimit = GetTasksLimit();
                    }
                    Console.WriteLine("Введите команду:");
                    BotCommand = Console.ReadLine() ?? "";
                    switch (BotCommand)
                    {
                        case "/start":
                            UserName = Start();
                            break;
                        case "/help":
                            Help(UserName);
                            break;
                        case "/info":
                            Info(UserName);
                            break;
                        case "/exit":
                            run = false;
                            break;
                        case "/addtask":
                            AddTask();
                            break;
                        case "/showtask":
                            ShowTasks();
                            break;
                        case "/removetask":
                            RemoveTask();
                            break;
                        case string bc when !string.IsNullOrWhiteSpace(UserName) && (bc == "/echo" || bc.StartsWith("/echo ")):
                            Echo(BotCommand, UserName);
                            break;
                        default:
                            NonCommand(BotCommand, UserName, InfoMessage);
                            break;
                    }
                }
                catch (DuplicateTaskException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (TaskCountLimitException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка:\n {ex.GetType()}\n {ex.Message}\n {ex.StackTrace}\n {ex.InnerException}");
                }
            }

        }

        /// <summary>
        /// Метод для начала сеанса взаимодействия бота с пользователм
        /// </summary>
        /// <returns>Имя пользователя</returns>
        static string Start()
        {
            Console.Write("Введите Ваше имя:");
            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Обработка введенной строк пользователем, которая не была распознана как команда.
        /// </summary>
        /// <param name="str">Введенная строка.</param>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="infoMessage">Сообщение для пользователя.</param>
        static void NonCommand(string str, string userName, string infoMessage)
        {
            if (!string.IsNullOrEmpty(str))
            {
                Console.WriteLine(Replay(userName, $"Команда {str} не предусмотрена к обработке."));
                Console.WriteLine(infoMessage);
            }
        }

        /// <summary>
        /// Команда возвращает введенный текст.
        /// </summary>
        /// <param name="botCommand">Введенная пользователем команда.</param>
        /// <param name="userName">Имя пользователя.</param>
        static void Echo(string botCommand, string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                Console.WriteLine(botCommand.Substring(6));
            }
        }

        /// <summary>
        /// Вывод информации о программе.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        static void Info(string userName)
        {
            Console.WriteLine(Replay(userName, "Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
        }

        /// <summary>
        ///  Формируется строка сообщения, при необходимости с обращением к пользователю в зависимости указано или нет имя пользователя. Если имя пользователя не задано,
        ///  тогда возвращаетя значение параметра message как есть. Иначе, возвращается строка с корректным обращением к пользователю
        /// </summary>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="message">Текст сообщения</param>
        /// <returns></returns>
        static string Replay (string UserName, string message)
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
        static void AddTask()
        {
            if (BotTasks.Count >= taskCountLimit)
            {
                throw new TaskCountLimitException((int)taskCountLimit);
            }
            Console.WriteLine("Введите описание задачи:");
            var description = Console.ReadLine() ?? "";
            if (!string.IsNullOrEmpty(description))
            {
                if (BotTasks.Contains(description))
                {
                    throw new DuplicateTaskException(description);
                }
                BotTasks.Add(description);
                Console.WriteLine("Задача добавлена.");
            }
        }

        /// <summary>
        /// Отображение списка задач
        /// </summary>
        static void ShowTasks()
        {
            if (BotTasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                for (int i = 0; i < BotTasks.Count; i++) {
                    Console.WriteLine($"{i+1}.{BotTasks[i]}");
                }
            }
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        static void RemoveTask()
        {
            ShowTasks();
            
            if (BotTasks.Count == 0) {
                Console.WriteLine("Нет задач к удалению.");
                return;
            }

            string TaskNumStr;

            string UnallowableNumMessage;

            Console.Write("Укажите номер задачи, которую Вы хотите удалить:");

            while (true)
            {
                TaskNumStr = Console.ReadLine() ?? "";
                UnallowableNumMessage = $"Недопустимое значение для номера задачи \"{TaskNumStr}\".\nВведите корректный номер задачи:";
                if (int.TryParse(TaskNumStr, out int TaskNum))
                {
                    if (TaskNum < 1 || TaskNum > BotTasks.Count)
                    {
                        Console.Write(UnallowableNumMessage);
                    }
                    else
                    {
                        BotTasks.RemoveAt(TaskNum - 1);
                        Console.WriteLine($"Задача под номером {TaskNum} удалена.");
                        break; // Выходим из бесконечного цикла
                    }
                }
                else
                {
                    Console.Write(UnallowableNumMessage);
                }
            }
        }
        /// <summary>
        /// Вывод информации о том, что делает бот - команда /help.
        /// </summary>
        static void Help(string UserName)
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
            Console.WriteLine(Replay(UserName, HelpMessage));

        }

        /// <summary>
        /// Выводит текст с запросом на ввод допустимого количества задач. Если введенное значение не входит в указанный диапазон значений, то генерируется исключение
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static int GetTasksLimit()
        {
            Console.Write($"Введите максимально допустимое количество задач ({minCountLimit}-{maxCountLimit}): ");
            string TasksLimitStr = Console.ReadLine() ?? "";
            return ParseAndValidateInt(TasksLimitStr, minCountLimit, maxCountLimit);
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
        static int ParseAndValidateInt(string? str, int min, int max)
        {
            ValidateString(str);

            if (!int.TryParse(str, out int TasksLimit) || TasksLimit < min || TasksLimit > max)
            {
                throw new ArgumentException($"Для максимального допустимого количества задач ожидалось значение от {min} до {max}, а было введено значение \"{str}\"");
            }
            return TasksLimit;
        }

        /// <summary>
        /// Проверка на "непустое" значение строки.
        /// </summary>
        /// <param name="str">Проверяемая строка</param>
        /// <exception cref="ArgumentException"></exception>
        static void ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException($"Недопустимое значение для строки - она не должна быть пустой и не должна содержать только пробельные символы");
            }
        }
    }

}
