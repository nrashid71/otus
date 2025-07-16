using LinqToDB.Mapping;

namespace Bot;

[Table("ToDoItem", Schema = "Infrastructure")]
public class ToDoItemModel
{
    [Column("Id"            , IsPrimaryKey = true )] public Guid      Id             { get; set; } // uuid
    [Column("Name"          , CanBeNull    = false)] public string    Name           { get; set; } = null!; // text
    [Column("ToDoUserId"                          )] public Guid      ToDoUserId     { get; set; } // uuid
    [Column("ToDoListId"                          )] public Guid?     ToDoListId     { get; set; } // uuid
    [Column("Deadline"                            )] public DateTime  Deadline       { get; set; } // timestamp (6) without time zone
    [Column("State"                               )] public int       State          { get; set; } // integer
    [Column("CreatedAt"                           )] public DateTime  CreatedAt      { get; set; } // timestamp (6) without time zone
    [Column("StateChangedAt"                      )] public DateTime? StateChangedAt { get; set; } // timestamp (6) without time zone

    #region Associations
    /// <summary>
    /// FK_ToDoItem_ToDoListId
    /// </summary>
    [Association(ThisKey = nameof(ToDoListId), OtherKey = nameof(ToDoList.Id))]
    public ToDoList? ToDoList { get; set; }

    /// <summary>
    /// FK_ToDoItem_ToDoUserId
    /// </summary>
    [Association(CanBeNull = false, ThisKey = nameof(ToDoUserId), OtherKey = nameof(ToDoUser.UserId))]
    public ToDoUser ToDoUser { get; set; } = null!;
    #endregion
}