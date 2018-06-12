using System;
using System.Threading.Tasks;

namespace Cynthia.Card.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("请输入你的名称~:");
            var name = Console.ReadLine();
            var content = "";
            //------------------------------------------------
            var client = new SignalRTest();
            await client.Start();
            await client.GetCacheMessage();
            while (true)
            {
                content = Console.ReadLine();
                client.SendMessage(name, content);
            }
        }
    }
}
