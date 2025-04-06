using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public class UpdateHandler : IUpdateHandler
{

    private List<string> _registredUserCommands = new List<string>() {"/addtask","/showtask","/removetask","/exit","/start"};
   
    public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
        IToDoService toDoService = new ToDoService();
        bool run = true;
        string botCommand;
        string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, exit. При вводе команды указываейте вначале символ / (слеш).";
        botClient.SendMessage(update.Message.Chat,"Здравствуйте!");
        botClient.SendMessage(update.Message.Chat,InfoMessage);
        while (run)
        {
            try
            {
                ((ToDoService)toDoService).checkTasks(botClient, update);
                botClient.SendMessage(update.Message.Chat,"Введите команду:");
                botCommand = Console.ReadLine() ?? "";
                switch (botCommand)
                {
                    case "/help":
                        ((ToDoService)toDoService).Help(botClient, update);
                        break;
                    case "/info":
                        ((ToDoService)toDoService).Info(botClient, update);
                        break;
                    default:
                        if (_registredUserCommands.Contains(botCommand))
                        {
                            if (((ToDoService)toDoService).toDoUser != null)
                            {
                                switch (botCommand)
                                {
                                    case "/start":
                                        ((ToDoService)toDoService).Start(botClient, update);
                                        break;
                                    case "/exit":
                                        run = false;
                                        break;
                                    case string bc when bc.StartsWith("/addtask "):
                                        ((ToDoService)toDoService).AddTask(botClient, update, botCommand.Substring("/addtask ".Length));
                                        break;                                    
                                    case "/showtask":
                                        ((ToDoService)toDoService).ShowTasks(botClient, update);
                                        break;
                                    case string bc when bc.StartsWith("/removetask "):
                                        ((ToDoService)toDoService).RemoveTask(botClient, update, botCommand.Substring("/removetask ".Length));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ((ToDoService)toDoService).NonCommand(botClient, update, botCommand, InfoMessage);
                        }
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

}