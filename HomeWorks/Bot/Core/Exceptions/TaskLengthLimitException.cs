namespace Bot
{
    internal class TaskLengthLimitException : ArgumentException
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}") { }
    }
}
