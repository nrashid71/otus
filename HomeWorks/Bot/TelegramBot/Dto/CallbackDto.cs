namespace Bot.Dto;

public class CallbackDto
{
    public string Action { get; set; }

    public static CallbackDto FromString(string input)
    {
        var dto = new CallbackDto();
        var array = input.Split('|');
        if (array.Length > 0)
        {
            dto.Action = array[0];
        }
        return dto;        
    }

    public override string ToString()
    {
        return Action;
    }
}