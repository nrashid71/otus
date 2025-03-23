namespace HomeWork05;

public class Stack
{
    /// <summary>
    /// Элемент стека.
    /// </summary>
    class StackItem
    {
        /// <summary>
        /// Текущее значение элемента стека
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// Ссылка на предыдущий элемент в стеке
        /// </summary>
        public StackItem Previous { set; get; }

        /// <summary>
        /// Конструктор. Инициализирует атрибуты класса
        /// </summary>
        /// <param name="value">Значение элемента стека</param>
        /// <param name="previous">Ссылка на предыдущий элемент стека</param>
        public StackItem(string value, StackItem previous)
        {
            Value = value;
            Previous = previous;
        }
        
    }

    private StackItem Head { set; get; }
    
    private int _count;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="values">params - параметр. Строки - элементы стека</param>
    public Stack(params string[] values)
    {
        foreach (var value in values)
        {
            Head = new StackItem(value, Head);
            _count++;
        }
    }
    
    /// <summary>
    /// Добавляет элемент в стек
    /// </summary>
    /// <param name="value">Добавляемое значение</param>
    public void Add(string value)
    {
        Head = new StackItem(value, Head);
        _count++;
    }

    /// <summary>
    /// Извлекает последний добавленный элемент и удаляет его из списка 
    /// </summary>
    /// <returns>Значение последнего добавленного элемента</returns>
    public string Pop()
    {
        if (Head == null)
        {
            throw new Exception("Стек пустой");
        }
        string value = Head.Value;
        
        Head = Head.Previous;
        
        _count--;
        
        return value;
    }
    
    /// <summary>
    ///  Количество элементов в стеке
    /// </summary>
    public int Size => _count;

    /// <summary>
    /// Значение верхнего элемента из стека. Если стек пустой - null
    /// </summary>
    /// <returns></returns>
    public string Top => Head?.Value;  
    
    /// <summary>
    /// Соединение стеков. Элементы каждого стека помещаются в результирующий стек в обратном порядке
    /// </summary>
    /// <param name="stacks">params - параметр. Значения параметров - стеки</param>
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