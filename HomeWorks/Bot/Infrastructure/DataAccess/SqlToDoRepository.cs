using LinqToDB;
using LinqToDB.Data;

namespace Bot;

public class SqlToDoRepository : IToDoRepository
{
    IDataContextFactory<ToDoDataContext> _dataContextFactory;
    
    public SqlToDoRepository(IDataContextFactory<ToDoDataContext> dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task<ToDoItem?> GetByGuid(Guid id, CancellationToken ct)
    {
        using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            var result = await dbContext
                                        .ToDoItems
                                        .LoadWith(i => i.ToDoUser)
                                        .LoadWith(i => i.ToDoList)
                                        .FirstOrDefaultAsync(i => i.Id == id);
            return result == null ? null : ModelMapper.MapFromModel(result);
        }
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                .ToDoItems
                .LoadWith(i => i.ToDoUser)
                .LoadWith(i => i.ToDoList)
                .Where(i => i.ToDoUser.UserId == userId)
                .Select(i => ModelMapper.MapFromModel(i))
                .ToList()
                .AsReadOnly();
        }
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                .ToDoItems
                .LoadWith(i => i.ToDoUser)
                .LoadWith(i => i.ToDoList)
                .Where(i => i.ToDoUserId == userId && i.State == (int)ToDoItemState.Active)
                .Select(i => ModelMapper.MapFromModel(i))
                .ToList()
                .AsReadOnly();
        }
    }

    public async Task Add(ToDoItem item, CancellationToken ct)
    {
        if ((await GetByGuid(item.Id, ct)) == null)
        {
            await using (var dbContext = _dataContextFactory.CreateDataContext())
            {
                await dbContext.InsertWithIdentityAsync(ModelMapper.MapToModel(item),token:ct);
                await dbContext.CommitTransactionAsync(ct);
            }
        }
        else
        {
            throw new Exception($"Задача с id {item.Id} уже существует");
        }
    }

    public async Task Update(ToDoItem item, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            await dbContext.InsertOrReplaceAsync(ModelMapper.MapToModel(item),token:ct);
            await dbContext.CommitTransactionAsync(ct);
        }
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            await dbContext.DeleteAsync(new ToDoItemModel(){ Id = id},token:ct);
            await dbContext.CommitTransactionAsync(ct);
        }
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext.ToDoItems.Any(i => i.ToDoUser.UserId == userId && i.Name == name);
        }
    }

    public async Task<int> CountActive(Guid userId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                    .ToDoItems
                    .LoadWith(i => i.ToDoUser)
                    .Count(i => i.ToDoUser.UserId == userId && i.State == (int)ToDoItemState.Active);
        }
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                    .ToDoItems
                    .LoadWith(i => i.ToDoUser)
                    .LoadWith(i => i.ToDoList)
                    .Where(t => t.ToDoUser.UserId == userId && predicate(ModelMapper.MapFromModel(t)))
                    .Select(i => ModelMapper.MapFromModel(i))
                    .ToList()
                    .AsReadOnly();
        }
    }
}