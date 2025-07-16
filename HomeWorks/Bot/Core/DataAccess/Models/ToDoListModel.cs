using LinqToDB.Mapping;

namespace Bot;

[Table("ToDoList", Schema = "Infrastructure")]
public class ToDoListModel
{
    [Column("Id"        , IsPrimaryKey = true )] public Guid     Id         { get; set; } // uuid
    [Column("Name"      , CanBeNull    = false)] public string   Name       { get; set; } = null!; // text
    [Column("ToDoUserId"                      )] public Guid     ToDoUserId { get; set; } // uuid
    [Column("CreatedAt"                       )] public DateTime CreatedAt  { get; set; } // timestamp (6) without time zone

    #region Associations
    /// <summary>
    /// FK_ToDoItem_ToDoListId backreference
    /// </summary>
    [Association(ThisKey = nameof(Id), OtherKey = nameof(ToDoItemModel.ToDoListId))]
    public IEnumerable<ToDoItem> ToDoItems { get; set; } = null!;

    /// <summary>
    /// FK_ToDoList_ToDoUserId
    /// </summary>
    [Association(CanBeNull = false, ThisKey = nameof(ToDoUserId), OtherKey = nameof(ToDoUser.UserId))]
    public ToDoUser ToDoUser { get; set; } = null!;
    #endregion
}