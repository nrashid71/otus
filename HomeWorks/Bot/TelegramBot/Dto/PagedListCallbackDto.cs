namespace Bot.Dto;

public class PagedListCallbackDto : ToDoListCallbackDto
{
    public int Page { get; set; }

    public ToDoItemState ToDoItemState { get; set; }
    PagedListCallbackDto()
    {
    }
    public PagedListCallbackDto(string action, Guid? toDoListId, int page, ToDoItemState toDoItemState)
    {
        Action = action;
        ToDoListId = toDoListId;
        Page = page;
        ToDoItemState = toDoItemState;
    }
    public static new PagedListCallbackDto FromString(string input)
    {
        var dto = new PagedListCallbackDto();
        var array = input.Split('|');
        if (array.Length > 0)
        {
            dto.Action = array[0];
        }
        if (array.Length > 1)
        {
            if (Guid.TryParse(array[1], out Guid val))
            {
                dto.ToDoListId = val;
            }
        }        
        if (array.Length > 2)
        {
            if (int.TryParse(array[2], out int val))
            {
                dto.Page = val;
            }
        }
        if (array.Length > 3)
        {
            if (Enum.TryParse(array[3], out ToDoItemState val))
            {
                dto.ToDoItemState = val;
            }
        }
        else
        {
            dto.ToDoItemState = ToDoItemState.Active;
        }
        return dto;        
    }

    public override string ToString()
    {
        return $"{base.ToString()}|{Page}|{ToDoItemState}";
    }

}