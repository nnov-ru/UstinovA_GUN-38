using System;

namespace Collections
{
    internal class Program
    {
        private class ListTask
        {
            private readonly List<string> _listOfStrings = new List<string> { "Roman", "Early Christian", "Celt"};
            public void TaskLoop()
            {
                Console.WriteLine($"Task 1. Here is the List:");
                WriteList();
                Console.WriteLine("Would you please input a new string to add to the end of the List:");
                _listOfStrings.Add(Console.ReadLine());
                Console.WriteLine($"Here is the new List:");
                WriteList();
                Console.WriteLine("Would you please input another new string to add just to the middle of the List:");
                _listOfStrings.Insert(_listOfStrings.Count / 2, Console.ReadLine());
                Console.WriteLine($"Here is the new List:");
                WriteList();

                Console.WriteLine("Would you please type '-exit' to return to the menu: ");
                while (Console.ReadLine() != "-exit")
                {
                    Console.WriteLine("Don't waste our time please : Gram the Program will proceed only after you type '-exit'.");
                }
            }
            private void WriteList()
            {
                foreach (var item in _listOfStrings)
                {
                    Console.WriteLine($"* {item}");
                }
                Console.WriteLine();
            }
        }

        private class LinkedListTask
        {
            private readonly Dictionary<string, float> _dictionary= new Dictionary<string, float>();
            public void TaskLoop()
            {
                Console.WriteLine("Task 2. Would you please input a student's name: ");
                var name = Console.ReadLine();
                float grade;
                do
                {
                    Console.WriteLine($"Would you please input {name}'s average grade (from 2 to 5):");
                } while (!float.TryParse(Console.ReadLine(), out grade) || grade <2f || grade >5f);
                
                _dictionary[name] = grade;

                Console.WriteLine("Student added ! Now would you please type the student's name to check whether I (Gram the Program) have correctly memorized their grade:");
                var nametocheck = Console.ReadLine();
                if (_dictionary.TryGetValue(nametocheck, out grade))
                    Console.WriteLine($"{name}'s average grade is {grade}.");
                else
                    Console.WriteLine($"Such a student named {nametocheck} doesn't exist as far as I know.");

                    Console.WriteLine("Would you please type '-exit' to return to the menu: ");
                while (Console.ReadLine() != "-exit")
                {
                    Console.WriteLine("Don't waste our time please : Gram the Program will proceed only after you type '-exit'.");
                }
            }
        }
        private class DoublyLinkedListTask
        {
            private class Node 
            {
                public string Value;
                public Node Next;
                public Node Previous;
            }
            private Node _home;
            private Node _end;
            
            public void TaskLoop()
            {
                Console.WriteLine("Task 3. Would you please create here a list of anything you are tired to keep in mind (counting from 3 to 6). ");
                Console.WriteLine("You please type - and I (Gram the Program) will memorize all this      for you.");
                Console.WriteLine("Would you please type '-exit' when the list is ready.");
                int count = 0;
                while (count < 6)
                {
                    Console.Write($"Item {count+1}: ");
                    var input = Console.ReadLine();
                    if (input == "-exit" && count >= 3) break;
                    if (input == "-exit") continue;

                    string value;
                    value = input;
                    var newnode = new Node { Value = value };
                    if (_home == null)
                    {
                        _home = newnode;
                        _end = newnode;
                    }
                    else
                    {
                        _end.Next = newnode;
                        newnode.Previous = _end;
                        _end = newnode;
                    }
                    count++;
                }
                Console.WriteLine("Now, let's have fun: I show you all I have memorized in the chronological order and vice-versa !");
                Console.WriteLine("Firstly, in the chronological order: ");
                var current1 = _home;
                while (current1 != null)
                {
                    Console.WriteLine($"* {current1.Value}");
                    current1 = current1.Next;
                }

                Console.WriteLine("And now, in the reversed one: ");
                var current2 = _end;
                while (current2 != null)
                {
                    Console.WriteLine($"* {current2.Value}");
                    current2 = current2.Previous;
                }

                Console.WriteLine("Thank you for watching ! Now would you please type '-exit' to return to the menu.");
                while (Console.ReadLine() != "-exit")
                {
                    Console.WriteLine("Don't waste our time please : Gram the Program will proceed only after you type '-exit'.");
                }
            }
        }

        static void Main(string[] args)
        {
            while (true) 
            {
                Console.WriteLine();
                Console.WriteLine("Enter 1, 2 or 3 to check Task 1, 2 or 3, respectively, OR type '-exit' to quit.");
                var input = Console.ReadLine();
                if (input == "-exit") break;
                if (!int.TryParse(input,out int task)) continue;
                switch (task)
                {
                    case 1:
                        CheckTask1();
                        break;
                    case 2:
                        CheckTask2();
                        break;
                    case 3:
                        CheckTask3();
                        break;
                }
            }
        }

        private static void CheckTask1()
        {
            var listTask = new ListTask();
            listTask.TaskLoop();
        }
        private static void CheckTask2()
        {
            var listTask = new LinkedListTask();
            listTask.TaskLoop();
        }
        private static void CheckTask3()
        {
            var listTask = new DoublyLinkedListTask();
            listTask.TaskLoop();
        }
    }
}