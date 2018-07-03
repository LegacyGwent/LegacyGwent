using System;
using System.Threading.Tasks;

namespace Cynthia.Card.Client.Desktop
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("请输入你的名称~:");
            var name = Console.ReadLine();
            var content = "";
            //------------------------------------------------
            var client = new ChatSignalR("http://132.232.106.125/hub/chat");
            await client.Start();
            await client.GetMessageCache();
            while (true)
            {
                content = Console.ReadLine();
                client.SendMessage(name, content);
            }
        }
    }
}
