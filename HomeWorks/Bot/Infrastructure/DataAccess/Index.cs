namespace Bot;

public class Index
{
    public Guid UserId{ get; set; }

    public Guid ItemId{ get; set; }

    public Index(Guid userId, Guid itemId)
    {
        UserId = userId;
        ItemId = itemId;
    }
}