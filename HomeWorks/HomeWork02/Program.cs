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
                        Console.WriteLine($"{username}, бот предоставляет краткую информацию по ключевым словам C# с небольшими примерами. Примеры ключевых слов abstract, event, namespace");
                        break;
                    case "/info":
                        Console.WriteLine($"{username}, версия программы 0.1.0-alpha. Дата создания 22.02.2025");
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
                        Console.WriteLine($"{username}, команда {BotCommand} не предусмотрена к обработке.");
                        Console.WriteLine(InfoMessage);
                        break;
                }
            }

        }
    }
}
