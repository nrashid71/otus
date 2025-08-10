using LinqToDB;

namespace Bot;

public class NotificationService : INotificationService
{
    IDataContextFactory<ToDoDataContext> _dataContextFactory;

    public NotificationService(IDataContextFactory<ToDoDataContext> dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    //Создает нотификацию. Если запись с userId и type уже есть, то вернуть false и не добавлять запись, иначе вернуть true
    public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            var recordIsExists  =  dbContext
                .Notification
                .LoadWith(i => i.ToDoUser)
                .Any(i => i.ToDoUser.UserId == userId && i.Type == type);
            if (recordIsExists)
            {
                return false;
            }
            await dbContext.InsertWithIdentityAsync(new NotificationModel()
            {
                Type = type,
                Text = text,
                ScheduledAt = scheduledAt,
                ToDoUserId = userId
            }, token:ct);
            return true;            
        }        
    }
    //Возвращает нотификации, у которых IsNotified = false && ScheduledAt <= scheduledBefore
    public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            return await dbContext
                .Notification
                .LoadWith(i => i.ToDoUser)
                .Where(i => !i.IsNotified && i.ScheduledAt <= scheduledBefore)
                .Select(i =>ModelMapper.MapFromModel(i))
                .ToListAsync(ct);
        }
    }

    public async Task MarkNotified(Guid notificationId, CancellationToken ct)
    {
        await using (var dbContext = _dataContextFactory.CreateDataContext())
        {
            NotificationModel notificationModel = dbContext
                .Notification
                .FirstOrDefaultAsync(i => i.Id == notificationId)
                .Result;
            if (notificationModel != null)
            {
                notificationModel.IsNotified = true;
                notificationModel.NotifiedAt = DateTime.UtcNow;
                await dbContext.UpdateAsync(notificationModel, token: ct);
                await dbContext.CommitTransactionAsync(ct);
            }
        }
    }
}