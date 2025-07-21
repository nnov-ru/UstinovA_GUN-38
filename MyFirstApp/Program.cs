using System.ComponentModel.Design;

internal class Program
{
    private static void Main(string[] args)
    {
        if (!Int32.TryParse(Console.ReadLine(), out var a))
        {  Console.WriteLine("ne chislo");
            return;
        }
        if (!Int32.TryParse(Console.ReadLine(), out var b))
        {
            Console.WriteLine("ne chislo");
            return;
        }

        var s = Console.ReadLine();
        var boolVar = true;
        if (s.Length == 0 || s.Length > 1 && !boolVar)
        {
           Console.WriteLine("neverny znak"); 
            return;
        }

        switch(s[0])
        {
            case '+':
                Console.WriteLine("Itog {0} + {1} = {2}",a,b,a+b);
                break;
            case '-':
                Console.WriteLine("Itog {0} - {1} = {2}", a, b, a - b);
                break;
            case '*':
                Console.WriteLine("Itog {0} x {1} = {2}", a, b, a * b);
                break;
            case '/':
                Console.WriteLine("Itog {0} / {1} = {2}", a, b, a / b);
                break;
            default:
                Console.WriteLine("Neverny znak");
                break;
        }    
    }
}