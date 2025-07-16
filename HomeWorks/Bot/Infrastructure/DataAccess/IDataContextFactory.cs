using LinqToDB.Data;

namespace Bot;

public interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
{
    TDataContext CreateDataContext();
}