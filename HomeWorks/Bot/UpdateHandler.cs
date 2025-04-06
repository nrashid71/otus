using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public class UpdateHandler : IUpdateHandler
{

    private List<string> _registredUserCommands = new List<string>() {"/addtask","/showtask","/removetask","/completetask","/showalltasks","/exit","/start"};

    private IToDoService _toDoService;
    public UpdateHandler()
    {
        _toDoService = new ToDoService(null);
    }
    public UpdateHandler(ToDoUser toDoUser)
    {
        _toDoService = new ToDoService(toDoUser);
    }

    public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
        string botCommand;
        string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, completetask, showalltasks, exit. При вводе команды указываейте вначале символ / (слеш).";
//        botClient.SendMessage(update.Message.Chat,"Здравствуйте!");
//        botClient.SendMessage(update.Message.Chat,InfoMessage);
        try
        {
            botClient.SendMessage(update.Message.Chat,"Введите команду:");
            botCommand = Console.ReadLine() ?? "";
            switch (botCommand)
            {
            case "/help":
                ((ToDoService)_toDoService).Help(botClient, update);
                break;
            case "/info":
                ((ToDoService)_toDoService).Info(botClient, update);
                break;
            default:
                var idx = botCommand.IndexOf(" ");
                if (_registredUserCommands.Contains(botCommand.Substring(0, idx == -1 ? botCommand.Length : idx).Trim()))
                {
                    if (((ToDoService)_toDoService).toDoUser != null)
                    {
                        switch (botCommand)
                        {
                            case "/start":
                                ((ToDoService)_toDoService).Start(botClient, update);
                                break;
                            case "/exit":
                                Environment.Exit(0);
                                break;
                            case string bc when bc.StartsWith("/addtask "):
                                ((ToDoService)_toDoService).AddTask(botClient, update,
                                    botCommand.Substring("/addtask ".Length));
                                break;
                            case "/showtask":
                                ((ToDoService)_toDoService).ShowTasks(botClient, update);
                                break;
                            case string bc when bc.StartsWith("/removetask "):
                                ((ToDoService)_toDoService).RemoveTask(botClient, update,
                                    botCommand.Substring("/removetask ".Length));
                                break;
                            case string bc when bc.StartsWith("/completetask "):
                                ((ToDoService)_toDoService).CompleteTask(
                                    botCommand.Substring("/completetask ".Length));
                                break;
                            case "/showalltasks":
                                ((ToDoService)_toDoService).ShowAllTasks(botClient, update);
                                break;
                        }
                    }
                }
                else
                {
                    ((ToDoService)_toDoService).NonCommand(botClient, update, botCommand,
                        InfoMessage);
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