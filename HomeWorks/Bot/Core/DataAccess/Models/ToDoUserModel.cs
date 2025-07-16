using LinqToDB.Mapping;

namespace Bot;

[Table("ToDoUser", Schema = "Infrastructure")]
public class ToDoUserModel
{
    [Column("UserId"          , IsPrimaryKey = true)] public Guid     UserId           { get; set; } // uuid
    [Column("TelegramUserId"                       )] public long     TelegramUserId   { get; set; } // bigint
    [Column("TelegramUserName"                     )] public string?  TelegramUserName { get; set; } // text
    [Column("RegisteredAt"                         )] public DateTime RegisteredAt     { get; set; } // timestamp (6) without time zone

    #region Associations
    /// <summary>
    /// FK_ToDoItem_ToDoUserId backreference
    /// </summary>
    [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoItemModel.ToDoUserId))]
    public IEnumerable<ToDoItem> ToDoItems { get; set; } = null!;

    /// <summary>
    /// FK_ToDoList_ToDoUserId backreference
    /// </summary>
    [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoListModel.ToDoUserId))]
    public IEnumerable<ToDoList> ToDoLists { get; set; } = null!;
    #endregion
}