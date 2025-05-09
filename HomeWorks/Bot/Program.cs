﻿using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IToDoRepository inMemoryToDoRepository = new InMemoryToDoRepository();
            IToDoService toDoService = new ToDoService(inMemoryToDoRepository);

            IUserRepository inMemoryUserRepository = new InMemoryUserRepository();
            IUserService userService = new UserService(inMemoryUserRepository);

            var handler = new UpdateHandler(toDoService, userService);
            try
            {
                handler.UpdateStarted += handler.OnHandleUpdateStarted;
                handler.UpdateCompleted += handler.OnHandleUpdateCompleted;

                var botClient = new ConsoleBotClient();

                var ct = new CancellationTokenSource();
                botClient.StartReceiving(handler, ct.Token);
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
