using LinqToDB;
using LinqToDB.Data;

namespace Bot;

public class ToDoDataContext : DataConnection
{
    private readonly IDataContext _dataContext;

    public ITable<ToDoItemModel> ToDoItems => _dataContext.GetTable<ToDoItemModel>();
    public ITable<ToDoListModel> ToDoLists => _dataContext.GetTable<ToDoListModel>();
    public ITable<ToDoUserModel> ToDoUsers => _dataContext.GetTable<ToDoUserModel>();

    public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL15, connectionString)
    {
        _dataContext = this;
    }
}