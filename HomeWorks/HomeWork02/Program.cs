namespace HomeWork02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            string BotCommand;
            string InfoMessage = "Вам доступны команды: start, help, info, exit. При вводе команды указываейте вначале симво / (слеш).";
            string username = "";
            Console.WriteLine("Здравствуйте!");
            Console.WriteLine(InfoMessage);
            while (run) {
                Console.WriteLine("Введите команду:");
                BotCommand = Console.ReadLine();
                switch (BotCommand) {
                    case "/start":
                        Console.WriteLine("Введите Ваше имя:");
                        username = Console.ReadLine();
                        break;
                    case "/help":
                        Console.WriteLine(reply(username, "Бот предоставляет краткую информацию по ключевым словам C# с небольшими примерами. Примеры ключевых слов abstract, event, namespace"));
                        break;
                    case "/info":
                        Console.WriteLine(reply(username, "Версия программы 0.1.0-alpha. Дата создания 22.02.2025."));
                        break;
                    case "/exit":
                        username = "";
                        run = false;
                        break;
                    case string bc when bc.StartsWith("/echo "):
                            if (!string.IsNullOrEmpty(username))
                            {
                                Console.WriteLine(BotCommand.Substring(6));
                            }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(BotCommand))
                        {
                            Console.WriteLine(reply(username, $"Команда {BotCommand} не предусмотрена к обработке."));
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
        /// <param name="username">Имя пользователя</param>
        /// <param name="message">Текст сообщения</param>
        /// <returns></returns>
        static string reply (string username, string message)
        {
            if (string.IsNullOrEmpty(username))
            {
                return message;
            }
            return $"{username}, " + message?.First().ToString().ToLower() + message?.Substring(1);
        }
    }
}
