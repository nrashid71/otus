using System.Runtime.CompilerServices;

namespace Bot.Dto;

public class ToDoListCallbackDto : CallbackDto
{
    public Guid? ToDoListId { get; set; }

    public ToDoListCallbackDto()
    {
    }
    public ToDoListCallbackDto(string action, Guid? toDoListId = null)
    {
        Action = action;
        ToDoListId = toDoListId;
    }
    public static new ToDoListCallbackDto FromString(string input)
    {
        var dto = new ToDoListCallbackDto();
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
        return dto;
    }
    public override string ToString()
    {
        return $"{base.ToString()}|{ToDoListId}";
    }
}