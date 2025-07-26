
namespace Bot;

public class DataContextFactory : IDataContextFactory<ToDoDataContext>
{
    private string _connectionString;
    public DataContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ToDoDataContext CreateDataContext()
    {
        return new ToDoDataContext(_connectionString);
    }
}