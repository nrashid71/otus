using LinqToDB;
using LinqToDB.Data;

namespace Bot;

public class ToDoDataContext : DataConnection
{
    private readonly IDataContext _dataContext;

    public ITable<ToDoItem> ToDoItems => _dataContext.GetTable<ToDoItem>();
    public ITable<ToDoList> ToDoLists => _dataContext.GetTable<ToDoList>();
    public ITable<ToDoUser> ToDoUsers => _dataContext.GetTable<ToDoUser>();

    public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString)
    {
        _dataContext = this;
    }
}