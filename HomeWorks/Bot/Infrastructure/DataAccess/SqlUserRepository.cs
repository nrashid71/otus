using LinqToDB;

namespace Bot;

public class SqlUserRepository : IUserRepository
{
    IDataContextFactory<ToDoDataContext> _dataContextFactory;

    public SqlUserRepository(IDataContextFactory<ToDoDataContext> dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            var r = dbContext.ToDoUsers.FirstOrDefault(i => i.UserId == userId);
            return r == null ? null : ModelMapper.MapFromModel(r);
        }
    }

    public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            var r = await dbContext.ToDoUsers.FirstOrDefaultAsync(i => i.TelegramUserId == telegramUserId);
            return r == null ? null : ModelMapper.MapFromModel(r);
        }
    }

    public async Task Add(ToDoUser user, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            await dbContext.InsertWithIdentityAsync(ModelMapper.MapToModel(user),token:ct);
            await dbContext.CommitTransactionAsync(ct);
        }
    }
    public async Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return dbContext
                .ToDoUsers
                .Select(i =>ModelMapper.MapFromModel(i))
                .ToList()
                .AsReadOnly();
        }
    }
}