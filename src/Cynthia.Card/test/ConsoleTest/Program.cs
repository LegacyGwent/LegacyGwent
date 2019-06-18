using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card;
using Cynthia.Card.Server;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            foreach (var item in 1.To(10))
            {
                if (item == 7) continue;
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}
