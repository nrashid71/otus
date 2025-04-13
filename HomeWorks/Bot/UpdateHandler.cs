using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Bot;

public class UpdateHandler : IUpdateHandler
{

    private List<string> _registredUserCommands = new List<string>() {"/addtask","/showtask","/removetask","/completetask","/showalltasks","/exit","/start","/report","/find"};

    private IToDoService _toDoService  = new ToDoService();
    public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
        string botCommand;
        string InfoMessage = "Вам доступны команды: start, help, info, addtask, showtasks, removetask, completetask, showalltasks, report, find, exit. При вводе команды указываейте вначале символ / (слеш).";
//        botClient.SendMessage(update.Message.Chat,"Здравствуйте!");
//        botClient.SendMessage(update.Message.Chat,InfoMessage);
        var toDoUser = ((ToDoService)_toDoService).UserService.GetUser(update.Message.From.Id);
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
            case "/start":
                ((ToDoService)_toDoService).Start(botClient, update);
                break;
            default:
                var idx = botCommand.IndexOf(" ");
                if (_registredUserCommands.Contains(botCommand.Substring(0, idx == -1 ? botCommand.Length : idx).Trim()))
                {
                    if ( toDoUser != null)
                    {
                        switch (botCommand)
                        {
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
                            case "/report":
                                ((ToDoService)_toDoService).Report(botClient, update);
                                break;
                            case string bc when bc.StartsWith("/find "):
                                ((ToDoService)_toDoService).Find(botClient, update, botCommand.Substring("/find ".Length));
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