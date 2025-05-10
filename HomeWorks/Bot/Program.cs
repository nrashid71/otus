using Telegram.Bot;

namespace Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
//            IToDoRepository inMemoryToDoRepository = new InMemoryToDoRepository();
            IToDoRepository toDoRepository = new FileToDoRepository(Path.Combine(Directory.GetCurrentDirectory(), "FileToDoRepository"));
            IToDoService toDoService = new ToDoService(toDoRepository);

            IUserRepository inMemoryUserRepository = new InMemoryUserRepository();
            IUserService userService = new UserService(inMemoryUserRepository);

            UpdateHandler handler = new UpdateHandler(toDoService, userService);
            
            string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");  // в linux для обычного пользователя
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Bot token not found. Please set the TELEGRAM_BOT_TOKEN environment variable.");
                return;
            }

            try
            {
                handler.UpdateStarted += handler.OnHandleUpdateStarted;
                handler.UpdateCompleted += handler.OnHandleUpdateCompleted;

                var botClient = new TelegramBotClient(token);

                var ct = new CancellationTokenSource();
                botClient.StartReceiving(handler, cancellationToken:ct.Token);
                while (true)
                {
                    Console.WriteLine("Нажмите клавишу A для выхода");
                    var s = Console.ReadLine();
                    if (s?.ToUpper() == "A")
                    {
                        ct.Cancel();
                        Console.WriteLine("Бот остановлен");
                        break;
                    }
                    else
                    {
                        var me = botClient.GetMe();
                        Console.WriteLine($"Бот запущен - {me.Result.Id}, {me.Result.FirstName}"); 
                    }
                }              
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Произошла непредвиденная ошибка:\n {ex.GetType()}\n {ex.Message}\n {ex.StackTrace}\n {ex.InnerException}");
            }
            finally
            {
                handler.UpdateStarted -= handler.OnHandleUpdateStarted;
                handler.UpdateCompleted -= handler.OnHandleUpdateCompleted;
            }
        }
    }
}
