namespace Bot
{
    internal class TaskCountLimitException : ArgumentException
    {
        public TaskCountLimitException(int taskCountLimit):base($"Превышено максимальное количество задач равное { taskCountLimit}") { }
    }
}
