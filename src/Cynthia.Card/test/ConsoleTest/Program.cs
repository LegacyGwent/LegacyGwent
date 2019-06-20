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
            ITestA test = new TestClass();
            Test(test);
            Console.ReadLine();
        }
        public static void Test<T>(T item)
        {
            Console.WriteLine(item is ITestB);
        }
        public interface ITestA { }
        public interface ITestB { }
        public class TestClass : ITestA, ITestB { }
    }
}
