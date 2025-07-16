
namespace Bot;

public class DataContextFactory : IDataContextFactory<ToDoDataContext>
{
    public ToDoDataContext CreateDataContext()
    {
        return new ToDoDataContext(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }
}