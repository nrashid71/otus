using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataContextFactory dataContextFactory = new DataContextFactory(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
//            IToDoRepository inMemoryToDoRepository = new InMemoryToDoRepository();
//            IToDoRepository toDoRepository = new FileToDoRepository(Path.Combine(Directory.GetCurrentDirectory(), "FileToDoRepository"));
            IToDoRepository toDoRepository = new SqlToDoRepository(dataContextFactory);
            IToDoService toDoService = new ToDoService(toDoRepository);

//            IUserRepository inMemoryUserRepository = new InMemoryUserRepository();
//            IUserRepository userRepository = new FileUserRepository(Path.Combine(Directory.GetCurrentDirectory(), "FileUserRepository"));
            IUserRepository userRepository = new SqlUserRepository(dataContextFactory);
            IUserService userService = new UserService(userRepository);

//            IToDoListRepository toDoListRepository = new FileToDoListRepository(Path.Combine(Directory.GetCurrentDirectory(), "FileToDoListRepository"));
            IToDoListRepository toDoListRepository = new SqlToDoListRepository(dataContextFactory);
            IToDoListService toDoListService = new ToDoListService(toDoListRepository);
            
            IScenarioContextRepository contextRepository = new InMemoryScenarioContextRepository();
            IScenario[] scenarios = new IScenario[]
            {
                new AddTaskScenario(userService, toDoListService, toDoService),
                new AddListScenario(userService, toDoListService),
                new DeleteListScenario(userService, toDoListService, toDoService),
                new DeleteTaskScenario(toDoService)
            };

            
            UpdateHandler handler = new UpdateHandler(toDoService, userService, scenarios, contextRepository, toDoListService);
            
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
                botClient.StartReceiving(handler, cancellationToken:ct.Token,
                    receiverOptions: new(){AllowedUpdates=[UpdateType.Message, UpdateType.CallbackQuery, UpdateType.Unknown]}
                    );
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
