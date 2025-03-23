using System.Xml;

namespace HomeWork05;

class Program
{
    static void Main(string[] args)
    {
        var s = new Stack("a", "b", "c");

        // size = 3, Top = 'c'
        Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

        var deleted = s.Pop();

        // Извлек верхний элемент 'c' Size = 2
        Console.WriteLine($"Извлек верхний элемент '{deleted}' Size = {s.Size}");
        s.Add("d");

        // size = 3, Top = 'd'
        Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

        s.Pop();
        s.Pop();
        s.Pop();
        // size = 0, Top = null
        Console.WriteLine($"size = {s.Size}, Top = {(s.Top == null ? "null" : s.Top)}");
        
        try
        {
            s.Pop();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"При попытке извлечения элемента из стека получили исключение: {ex.Message}");
        }
        // Доп. задание 1
        Console.WriteLine("Доп. задание 1");
        s = new Stack("a", "b", "c");
        s.Merge(new Stack("1", "2", "3"));
        while (s.Size > 0)
        {
            Console.WriteLine(s.Pop());
        }        
        // Доп. задание 2
        Console.WriteLine("Доп. задание 2");
        s = Stack.Concat(new Stack("a", "b", "c"), new Stack("1", "2", "3"), new Stack("А", "Б", "В"));
        while (s.Size > 0)
        {
            Console.WriteLine(s.Pop());
        }        
        
    }
}