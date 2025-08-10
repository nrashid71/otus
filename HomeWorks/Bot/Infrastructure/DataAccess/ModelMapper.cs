namespace Bot;

internal static class ModelMapper
{
    public static ToDoUser MapFromModel(ToDoUserModel model)
    {
        return new ToDoUser()
        {
            UserId = model.UserId,
            TelegramUserId =  model.TelegramUserId,
            TelegramUserName =  model.TelegramUserName,
            RegisteredAt =  model.RegisteredAt
        };
    }

    public static ToDoUserModel MapToModel(ToDoUser entity)
    {
        return new ToDoUserModel()
        {
            UserId = entity.UserId,
            TelegramUserId = entity.TelegramUserId,
            TelegramUserName = entity.TelegramUserName,
            RegisteredAt = entity.RegisteredAt
        };
    }

    public static ToDoItem MapFromModel(ToDoItemModel model)
    {
        return new ToDoItem()
        {
            Id = model.Id,
            Name =  model.Name,
            ToDoUser = model.ToDoUser,
            List = model.ToDoList,
            Deadline =  model.Deadline,
            CreatedAt = model.CreatedAt,
            StateChangedAt =  model.StateChangedAt,
            State =  (ToDoItemState) model.State
        };
    }

    public static ToDoItemModel MapToModel(ToDoItem entity)
    {
        return new ToDoItemModel()
        {
            Id = entity.Id,
            Name = entity.Name,
            ToDoUserId = entity.ToDoUser.UserId,
            ToDoListId = entity.List?.Id,
            Deadline = entity.Deadline,
            CreatedAt = entity.CreatedAt,
            StateChangedAt = entity.StateChangedAt,
            State = (int)entity.State
        };
    }

    public static ToDoList MapFromModel(ToDoListModel model)
    {
        return new ToDoList()
        {
            Id = model.Id,
            Name = model.Name,
            CreatedAt = model.CreatedAt,
            ToDoUser = model.ToDoUser
        };
    }

    public static ToDoListModel MapToModel(ToDoList entity)
    {
        return new ToDoListModel()
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            ToDoUserId = entity.ToDoUser.UserId
        };
    }
    
    public static Notification MapFromModel(NotificationModel model)
    {
        return new Notification()
        {
            Id = model.Id,
            ToDoUser = model.ToDoUser,
            Type = model.Type,
            Text = model.Text,
            ScheduledAt = model.ScheduledAt,
            IsNotified = model.IsNotified,
            NotifiedAt = model.NotifiedAt
        };
    }
    
    public static NotificationModel MapToModel(Notification entity)
    {
        return new NotificationModel()
        {
            Id = entity.Id,
            ToDoUser = entity.ToDoUser,
            Type = entity.Type,
            Text = entity.Text,
            ScheduledAt = entity.ScheduledAt,
            IsNotified = entity.IsNotified,
            NotifiedAt = entity.NotifiedAt
        };
    }
}