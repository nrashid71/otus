namespace HomeWork05;

public class Stack
{
    private List<string> _stack;

    public Stack(params string[] values)
    {
        _stack = new List<string>();
        _stack.AddRange(values);
    }
    
    /// <summary>
    /// Добавляет элемент в стек
    /// </summary>
    /// <param name="value">Добавляемое значение</param>
    public void Add(string value)
    {
        _stack.Add(value);
    }

    /// <summary>
    /// Извлекает последний добавленный элемент и удаляет его из списка 
    /// </summary>
    /// <returns>Значение последнего добавленного элемента</returns>
    public string Pop()
    {
        if (Size== 0)
        {
            throw new Exception("Стек пустой");
        }
        var headIndex = Size - 1;
        var value = _stack[headIndex];
        _stack.RemoveAt(headIndex);
        return value;
    }
    
    /// <summary>
    ///  Количество элементов в стеке
    /// </summary>
    public int Size => _stack.Count;
    
    /// <summary>
    /// Значение верхнего элемента из стека. Если стек пустой - null
    /// </summary>
    /// <returns></returns>
    public string Top =>  Size> 0 ?  _stack[Size- 1] : null;
    
    /// <summary>
    /// Соединение стеков. Элементы каждого стека помещаются в результирующий стек в обратном порядке
    /// </summary>
    /// <param name="stacks">Последовательность параметров - стеки</param>
    /// <returns></returns>
    public static Stack Concat(params Stack[] stacks)
    {
        Stack stack = new Stack();
        foreach (var s in stacks)
        {
            stack.Merge(s);
        }
        return stack;
    }
}