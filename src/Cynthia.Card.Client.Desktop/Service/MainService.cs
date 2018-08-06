using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Utilities.LifetimeAnnotations;
using Alsein.Utilities;

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
            Console.Title = "登录界面";
            Console.Clear();
            Console.SetCursorPosition(30, 3);
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
            Console.WriteLine("得到结果");
            if (Client.User.PlayerName == null)
            {
                Console.WriteLine("登录失败\n用户名或密码错误,或者用户已经处于登录状态,请重新尝试\n~按下任意键返回~");
                Console.ReadKey();
                goto start;
            }
        menu:
            Console.Clear();
            Console.WriteLine($"登录成功~,欢迎回来{Client.User.PlayerName},您有{Client.User.Decks.Count}套卡组\n");
            Console.WriteLine($"按下1进行匹配");
            Console.WriteLine($"按下2退出游戏");
            op = Console.ReadLine();
            if (op == "1")
                goto match;
            if (op == "2")
                return;
            goto menu;
        match:
            Console.Clear();
            if (Client.User.Decks.Count <= 0)
            {
                Console.WriteLine($"当前没有卡组,无法进行匹配");
                Console.WriteLine($"~按下任意键返回~");
                Console.ReadKey();
                goto menu;
            }
            Console.WriteLine($"使用第几套卡组进行匹配?");
            try
            {
                var deckIndex = int.Parse(Console.ReadLine());
                if (deckIndex > 0 && deckIndex <= Client.User.Decks.Count)
                {
                    if (!await Client.Match(deckIndex - 1))
                    {
                        Console.WriteLine($"匹配发生了一些错误...匹配失败");
                        Console.WriteLine($"~按下任意键返回~");
                        Console.ReadKey();
                        goto menu;
                    }
                    Console.WriteLine($"匹配中~请稍等");
                    var game = Container.Resolve<GwentClientGameService>();
                    var playTask = game.Play(Client.Player);
                    await playTask;
                    Console.WriteLine($"~比赛结束~");
                    Console.WriteLine($"~按下任意键返回~");
                    Console.ReadKey();
                    goto menu;
                }
                Console.WriteLine($"没有找到您选择的卡组,请检查后重新输入\n");
                Console.WriteLine($"~按下任意键返回~\n");
                Console.ReadKey();
                goto menu;
            }
            catch
            {
                Console.WriteLine($"输入错误...请输入大于0,小于等于您当前拥有卡组数的整数\n");
                Console.WriteLine($"~按下任意键返回~\n");
                Console.ReadKey();
                goto menu;
            }

        }
    }
}