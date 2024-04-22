using System;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            var N = int.Parse(Console.ReadLine());
            for(int i=0;i<N;i++)
            {
                var line = Console.ReadLine();
                Scanner scanner = new Scanner(line);
                Parser parser = new Parser(scanner);
                var value = parser.E();
                if(parser.Error)
                    Console.WriteLine("ERROR");
                else
                {
                    Console.WriteLine(value);
                    Console.Write("Rules: ");
                    foreach (var rule in parser.Rules)
                    {
                        Console.Write(rule + " ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
