namespace Bot;

public class Notification
{
    public Guid Id { get; set; }

    public ToDoUser ToDoUser { get; set; }

    public string Type { get; set; } //Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}
    
    public string Text { get; set; } //Текст, который будет отправлен
    
    public DateTime ScheduledAt { get; set; } //Запланированная дата отправки
    
    public bool IsNotified { get; set; } //Флаг отправки
    
    public DateTime? NotifiedAt { get; set; } //Фактическая дата отправки
}