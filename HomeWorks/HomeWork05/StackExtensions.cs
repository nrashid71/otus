using System.Data;

namespace HomeWork05;

/// <summary>
/// Класс расширения для класса Stack 
/// </summary>
public static class StackExtensions
{
    /// <summary>
    /// Метод для добавления элементов "другого" стека (источник новых элементов) в обратном порядке
    /// Особенность: "другой" стек по завершению работы метода "опустошается" 
    /// </summary>
    /// <param name="stack">Параметр this</param>
    /// <param name="other">Стек, элементы которого "вливаются" в текущий стек</param>
    public static void Merge(this Stack stack, Stack other)
    {
        while (other.Size > 0)
        {
            stack.Add(other.Pop());
        }
    }
}