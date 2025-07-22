namespace Arrays {
    internal class Program
    {
        static void Main(string[] args)
        {
            var a1 = new int[8] { 0, 1, 1, 2, 3, 5, 8, 13 };
            Console.WriteLine(string.Join(", ", a1));

            var a2 = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            Console.WriteLine(string.Join(", ", a2));

            var a3 = new int[3, 3] {
                        {2, 3, 4 },
                        {4, 9, 16 },
                        {8, 27, 64 }
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(a3[i, j]+ " ");
                }
                Console.WriteLine();
            }
            var a4 = new double[3][] {
            new double[5] {1, 2, 3, 4, 5 },
            new double[2] {Math.E, Math.PI },
            new double[4] {Math.Log10(1), Math.Log10(10), Math.Log10(100), Math.Log10(1000) }
            };
            Console.WriteLine(string.Join(", ", a4[0]));
            Console.WriteLine(string.Join(", ", a4[1]));
            Console.WriteLine(string.Join(", ", a4[2]));

            int[] array = { 1, 2, 3, 4, 5 };
            int[] array2 = { 7, 8, 9, 10, 11, 12, 13 };
            var result = CopyArrays(array, array2, 3);
            Console.WriteLine(string.Join(", ", result));

            //string[] sample = { "", "" };
            ResizeArray(ref array, 10);
            Console.WriteLine(string.Join(", ", array));
        }
        static int[] CopyArrays(int[] source, int[] destination, int count)
        { 
        Array.Copy(source, destination, count);
        return destination;
        }
        static int[] ResizeArray(ref int[] oldarray, int newSize)
        {
        Array.Resize(ref oldarray, newSize);
        return oldarray;
        }
    }
}