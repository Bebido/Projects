using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellocsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Hello World!");
            
            string napis = "Andrzej" + "Duda";
            Console.WriteLine(napis);
            
            string a = napis.ToUpper();
            Console.WriteLine(a);
            Console.ReadKey();
        }
    }
}
