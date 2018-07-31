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
            await Client.StartAsync();
        start:
            Console.Clear();
            Console.WriteLine("~这里是游戏功能测试程序主菜单~");
            Console.WriteLine("按下1进行注册\n按下2进行登录");
            var op = Console.ReadLine();
            if (op == "1")
                goto register;
            if (op == "2")
                goto login;
            goto start;
        register:
            Console.Clear();
            Console.WriteLine("~~~~开始进行注册~~~~\n请输入注册的用户名:");
            var rusername = Console.ReadLine();
            Console.WriteLine("请输入您在游戏中的昵称");
            var rplayname = Console.ReadLine();
            Console.WriteLine("请输入注册密码:");
            var rpassword = Console.ReadLine();
            Console.WriteLine("请再次输入密码:");
            if (rpassword != Console.ReadLine())
            {
                Console.WriteLine("两次密码不一致,注册失败...按下任意键返回");
                Console.ReadKey();
                goto start;
            }
            Console.WriteLine("正在验证...请等待");
            if (await Client.Register(rusername, rpassword, rplayname))
            {
                Console.WriteLine("注册成功~按下任意键返回");
                Console.ReadKey();
                goto start;
            }
            Console.WriteLine("注册失败...用户名或者昵称重名,请尝试使用其他名称或用户名进行注册");
            Console.WriteLine("~按下任意键返回~");
            Console.ReadKey();
            goto start;
        login:
            Console.Clear();
            Console.WriteLine("~~~~开始进行登录~~~~\n请输入您的用户名:");
            var lusername = Console.ReadLine();
            Console.WriteLine("请输入您的密码");
            var lpassword = Console.ReadLine();
            Console.WriteLine("正在验证...请等待");
            var user = await Client.Login(lusername, lpassword);
            if (user == null)
            {
                Console.WriteLine("登录失败\n用户名或密码错误,或者用户已经处于登录状态,请重新尝试\n~按下任意键返回~");
                Console.ReadKey();
                goto start;
            }
            Console.WriteLine($"登录成功~,欢迎回来{user.PlayerName},您有{user.Decks.Count}套卡组\n");
            if (await Client.Match(1)) Console.WriteLine("成功匹配");
            var game = Container.Resolve<GwentClientGameService>();
            var play = game.Play(Client.Player);
            Console.ReadKey();
        }
    }
}