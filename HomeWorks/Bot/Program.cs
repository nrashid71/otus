using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var handler = new UpdateHandler();
                var botClient = new ConsoleBotClient();
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка:\n {ex.GetType()}\n {ex.Message}\n {ex.StackTrace}\n {ex.InnerException}");
            }
        }
    }
}
