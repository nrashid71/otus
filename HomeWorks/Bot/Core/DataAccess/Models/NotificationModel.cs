using LinqToDB.Mapping;

namespace Bot;

		[Table("Notification", Schema = "Infrastructure")]
		public class NotificationModel
		{
			[Column("Id"         , IsPrimaryKey = true )] public Guid      Id          { get; set; } // uuid
			[Column("ToDoUserId"                       )] public Guid      ToDoUserId  { get; set; } // uuid
			[Column("Type"       , CanBeNull    = false)] public string    Type        { get; set; } = null!; // text
			[Column("Text"       , CanBeNull    = false)] public string    Text        { get; set; } = null!; // text
			[Column("ScheduledAt"                      )] public DateTime  ScheduledAt { get; set; } // timestamp (6) without time zone
			[Column("IsNotified"                       )] public bool      IsNotified  { get; set; } // boolean
			[Column("NotifiedAt"                       )] public DateTime? NotifiedAt  { get; set; } // timestamp (6) without time zone

			#region Associations
			/// <summary>
			/// FK_TNotification_ToDoUserId
			/// </summary>
			[Association(CanBeNull = false, ThisKey = nameof(ToDoUserId), OtherKey = nameof(ToDoUser.UserId))]
			public ToDoUser ToDoUser { get; set; } = null!;
			#endregion
		}
