using LinqToDB;
using LinqToDB.Data;

namespace Bot;

public class ToDoDataContext : DataConnection
{
    public ITable<ToDoItemModel> ToDoItems => this.GetTable<ToDoItemModel>();
    public ITable<ToDoListModel> ToDoLists => this.GetTable<ToDoListModel>();
    public ITable<ToDoUserModel> ToDoUsers => this.GetTable<ToDoUserModel>();
    public ITable<NotificationModel> Notification => this.GetTable<NotificationModel>();
    public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL15, connectionString)
    {
    }
}