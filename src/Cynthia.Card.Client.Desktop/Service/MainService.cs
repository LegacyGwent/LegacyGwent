using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    public class MainService
    {
        public GwentChatService Client { get; set; }
        public async Task Main(string[] args)
        {
            await Client.StartAsync();
            Console.WriteLine("请输入你的名称~:");
            var name = Console.ReadLine();
            var content = "";
            //------------------------------------------------
            await Client.GetMessageCache();
            while (true)
            {
                content = Console.ReadLine();
                Client.SendMessage(name, content);
            }
        }
    }
}