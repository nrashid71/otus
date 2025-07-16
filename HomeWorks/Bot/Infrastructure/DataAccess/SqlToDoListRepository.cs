using LinqToDB;

namespace Bot;

public class SqlToDoListRepository : IToDoListRepository
{
    IDataContextFactory<ToDoDataContext> _dataContextFactory;

    public SqlToDoListRepository(IDataContextFactory<ToDoDataContext> dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }
    public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            var result  = dbContext
                .ToDoLists
                .LoadWith(r => r.ToDoUser)
                .FirstOrDefault(i => i.Id == id);
            return result == null ? null : ModelMapper.MapFromModel(result);
        }
    }

    public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                .ToDoLists
                .LoadWith(r => r.ToDoUser)
                .Where(i => i.ToDoUser.UserId == userId)
                .Select(i => ModelMapper.MapFromModel(i))
                .ToList()
                .AsReadOnly();
        }
    }

    public async Task Add(ToDoList list, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            await dbContext.BeginTransactionAsync(ct);
            await dbContext.InsertWithIdentityAsync(ModelMapper.MapToModel(list),token:ct);
            await dbContext.CommitTransactionAsync(ct);
        }
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            await dbContext.BeginTransactionAsync(ct);
            await dbContext.DeleteAsync(new ToDoListModel(){ Id = id},token:ct);
            await dbContext.CommitTransactionAsync(ct);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext.ToDoLists.LoadWith(r => r.ToDoUser).Any(i => i.ToDoUser.UserId == userId && i.Name == name);
        }
    }
}