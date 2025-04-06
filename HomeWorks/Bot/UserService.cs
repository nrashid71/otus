namespace Bot;

public class UserService : IUserService
{
//    private List<ToDoUser> _toDoUsers  = new List<ToDoUser>();
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        ToDoUser toDoUser = new ToDoUser(telegramUserId, telegramUserName);
        
//        if (GetUser(telegramUserId) == null)
//        {
//            _toDoUsers.Add(toDoUser);
//        }
        
        return toDoUser;
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        throw new NotImplementedException();
        // return _toDoUsers.FirstOrDefault(t => t.TelegramUserId == telegramUserId)
    }
}