using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class MainService
    {
        public GwentChatService Chat { get; set; }
        public GwentClientService Client { get; set; }
        public IContainer Container { get; set; }
        public async Task Main(string[] args)
        {
            Console.WriteLine("开始执行");
            await Client.StartAsync();
            Console.WriteLine("输入用户名");
            if (await Client.Login(Console.ReadLine(), "123456")) Console.WriteLine("成功登录");
            if (await Client.Match(1)) Console.WriteLine("成功匹配");
            var game = Container.Resolve<GwentClientGameService>();
            var play = game.Play(Client.Player);
            Console.ReadKey();
        }
    }
}