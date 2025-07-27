namespace Cycles {
    internal class Program
    {
        static void Main(string[] args)
        {
            //chisla Fibonachchi
            var fibonacci10 = new int[10];
            fibonacci10[0] = 0;
            fibonacci10[1] = 1;
            for (int i = 2; i<10; i++)
            {
                fibonacci10[i] = fibonacci10[i-1]+fibonacci10[i-2];
            }
            Console.WriteLine(string.Join(" ", fibonacci10));

            //chetnie chisla
            for (int i = 2; i<21; i+=2)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            //tablica umnozhenia 5 x 5
            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                  Console.Write($"{i * j,3}");
                }
                Console.WriteLine();
            }

            //vvod parolia
            string password = "qwerty";
            string input;
            Console.WriteLine("Would you please enter your password:");
            do
            {
                    input = Console.ReadLine();
                    if (input != password)
                    {
                        Console.WriteLine("Incorrect password. Would you please try again:");
                    }
            } while (input != password);
            Console.WriteLine("Password is correct! Your memory is doing all right!");
        }
    }
}