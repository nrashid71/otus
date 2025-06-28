namespace Bot.Dto;

public class PagedListCallbackDto : ToDoListCallbackDto
{
    public int Page { get; set; }

    public ToDoItemState ToDoItemState { get; set; }
    public PagedListCallbackDto(string? action, Guid? toDoListId, int page, ToDoItemState toDoItemState)
    {
        Action = action;
        ToDoListId = toDoListId;
        Page = page;
        ToDoItemState = toDoItemState;
    }
    public static new PagedListCallbackDto FromString(string input)
    {
        var array = input.Split('|');
        return new PagedListCallbackDto(
            action: (array.Length > 0 ? array[0] : null),
            toDoListId: (array.Length > 1 ? (Guid.TryParse(array[1], out Guid val1) ? val1 : null) : null),
            page: (array.Length > 2 ? (int.TryParse(array[2], out int val2) ? val2 : 0) : 0),
            toDoItemState: (array.Length > 3 ? (Enum.TryParse(array[3], out ToDoItemState val3)? val3:ToDoItemState.Active) : ToDoItemState.Active));
    }

    public override string ToString()
    {
        return $"{base.ToString()}|{Page}|{ToDoItemState}";
    }

}