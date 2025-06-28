namespace Bot.Dto;

public class ToDoItemCallbackDto : CallbackDto
{
    public Guid ToDoItemId { get; set; }

    public static new ToDoItemCallbackDto FromString(string input)
    {
        var dto = new ToDoItemCallbackDto();
        var array = input.Split('|');
        if (array.Length > 0)
        {
            dto.Action = array[0];
        }
        if (array.Length > 1)
        {
            if (Guid.TryParse(array[1], out Guid val))
            {
                dto.ToDoItemId = val;
            }
        }
        return dto;
    }
    public override string ToString()
    {
        return $"{base.ToString()}|{ToDoItemId}";
    }    
}