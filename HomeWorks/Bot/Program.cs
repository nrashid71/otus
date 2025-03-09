using System.Threading.Tasks;

namespace Bot
{
    internal class Program
    {
        static List<string> BotTasks = new List<string>();
        static void Main(string[] args)
        {
            bool run = true;
            string BotCommand;
            string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, exit. При вводе команды указываейте вначале символ / (слеш).";
            string UserName = "";
            Console.WriteLine("Здравствуйте!");
            Console.WriteLine(InfoMessage);
            while (run) {
                Console.WriteLine("Введите команду:");
                BotCommand = Console.ReadLine() ?? "";
                switch (BotCommand) {
                    case "/start":
                        Console.Write("Введите Ваше имя:");
                        UserName = Console.ReadLine() ?? "";
                        break;
                    case "/help":
                        Help(UserName);
                        break;
                    case "/info":
                        Console.WriteLine(Replay(UserName, "Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
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
                    case string bc when bc.StartsWith("/echo "):
                            if (!string.IsNullOrEmpty(UserName))
                            {
                                Console.WriteLine(BotCommand.Substring(6));
                            }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(BotCommand))
                        {
                            Console.WriteLine(Replay(UserName, $"Команда {BotCommand} не предусмотрена к обработке."));
                            Console.WriteLine(InfoMessage);
                        }
                        break;
                }
            }

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
            Console.WriteLine("Введите описание задачи:");
            var description = Console.ReadLine() ?? "";
            if (!string.IsNullOrEmpty(description))
            {
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

    }
}
