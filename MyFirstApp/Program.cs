using System.ComponentModel.Design;

internal class Program
{
    private static void Main(string[] args)
    {
        while (true) 
        {
        Console.WriteLine("Peace be with you, Friend! It is Bit-by-Bit Calculator speaking. Would you please input the first DECIMAL number of your bit-by-bit calculation:"); 
        if (!int.TryParse(Console.ReadLine(), out var a))
        {
            Console.WriteLine("It's not a DECIMAL number, so either you've misread my greetings or you're making fun of me, wasting my time... Please try again, but only after restarting");
            return;
        }
        Console.WriteLine($"What you've inputted can be shown as a BINARY number: {Convert.ToString(a, 2)}");
        
        Console.WriteLine("Got you! Would you please input now the last DECIMAL number of your calculation:");
        if (!int.TryParse(Console.ReadLine(), out var b))
        {
            Console.WriteLine("It's not a DECIMAL number, so either you've misread my suggestion or you're making fun of me, wasting my time... Please try again, but only after restarting");
            return;
        }
        Console.WriteLine($"What you've inputted can be shown as a BINARY number: {Convert.ToString(b, 2)}");

        Console.WriteLine("Got you! Would you please give a sign of a logical bit-by-bit operation to be calculated.");
        Console.WriteLine("Please limit yourself by   &   |   ^   operation signs. Where:");
        Console.WriteLine("  & means bit-by-bit checking whether the inputted numbers are equal");
        Console.WriteLine("  | means bit-by-bit checking whether any of the inputted numbers' bits pairs contains truth");
        Console.WriteLine("  ^ means bit-by-bit checking whether the inputted numbers are not equal");
        var s = Console.ReadLine();      
        if (s != "&" && s != "|" && s != "^")
        { 
           Console.WriteLine("Wrong operation - there's a certain misunderstanding here now. Calculator is stopping working now. Please restart after you come to your senses!"); 
            return;
        }

            switch (s[0])
            {
                case '&':
                    Console.WriteLine($"Decimal Result of {a} & {b} = {a & b}");
                    Console.WriteLine($"Binary Result of {a} & {b} = {Convert.ToString(a & b, 2)}");
                    Console.WriteLine($"Hexadecimal Result of {a} & {b} = {(a & b):X}");
                    Console.WriteLine();
                    break;
                case '|':
                    Console.WriteLine($"Decimal Result of {a} | {b} = {a | b}");
                    Console.WriteLine($"Binary Result of {a} | {b} = {Convert.ToString(a | b, 2)}");
                    Console.WriteLine($"Hexadecimal Result of {a} | {b} = {(a | b):X}");
                    Console.WriteLine(); 
                    break;
                case '^':
                    Console.WriteLine($"Decimal Result of {a} ^ {b} = {a ^ b}");
                    Console.WriteLine($"Binary Result of {a} ^ {b} = {Convert.ToString(a ^ b, 2)}");
                    Console.WriteLine($"Hexadecimal Result of {a} ^ {b} = {(a ^ b):X}");
                    Console.WriteLine(); 
                    break;
                default:
                    Console.WriteLine("Wrong operation - there's a certain misunderstanding here now. Calculator is stopping working now. Please restart after you come to your senses!");
                    break;
            }
        }    
    }
}