using System.Threading.Tasks;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
using System;
using Alsein.Extensions;
using MongoDB.Driver;
using Cynthia.Card;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using Alsein.Extensions.Extensions;
using System.IO.Compression;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("欢迎使用卡组码小工具~\n");

            var IP = Dns.GetHostEntry("cynthia.ovyno.com").AddressList[0];
            var client = new HubConnectionBuilder().WithUrl($"http://{IP}:5005/hub/gwent").Build();
            try
            {
                Console.WriteLine("正在与服务器进行连接...请稍等片刻...");
                await client.StartAsync();
            }
            catch
            {
                Console.WriteLine("无法连接服务器,请稍后重试...");
                await GwentTest.ConfirmExit();
                return;
            }

            var userName = "";
            var passWord = "";
            var user = default(UserInfo);

            while (true)
            {
                Console.WriteLine("\n\n请先登录\n");
                Console.WriteLine("请输入用户名:");
                userName = Console.ReadLine();
                Console.WriteLine("请输入密码:");
                passWord = Console.ReadLine();

                user = await client.InvokeAsync<UserInfo>("Login", userName, passWord);
                if (user == null)
                {
                    Console.WriteLine("登录失败!");
                }
                else
                {
                    break;
                }
            }
            var chose = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"欢迎~{user.PlayerName},请输入数字选择功能:\n");
                Console.WriteLine("1.导入卡组");
                Console.WriteLine("2.导出卡组");
                Console.WriteLine("其他.退出程序");
                Console.WriteLine("\n请输入进行选择:");
                chose = Console.ReadLine();
                switch (chose)
                {
                    case "1":
                        Console.WriteLine("请输入卡组码:");
                        var deckCode = Console.ReadLine();
                        try
                        {
                            var deck = deckCode.DeCompressToDeck();
                            if (!deck.IsBasicDeck())
                            {
                                Console.WriteLine("该卡组无法用于对战,请录入完整卡组!(按下回车返回主菜单)");
                            }
                            Console.WriteLine("请输入录入卡组名称(1-20字符):");
                            var name = Console.ReadLine();
                            name = string.IsNullOrWhiteSpace(name) ? "默认卡组" : (name.Length >= 20 ? name[0..20] : name);
                            deck.Name = name;
                            Console.WriteLine("录入中...请稍等片刻...");
                            await client.InvokeAsync<bool>("AddDeck", deck);
                            Console.WriteLine($"完成录入~录入的卡组需要重启程序才能显示~");
                            Console.WriteLine("按下任意键返回主菜单");
                            Console.ReadLine();
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("发生错误,按下回车返回主菜单");
                            Console.ReadLine();
                            break;
                        }
                    case "2":
                        var decksShow = user.Decks.Select(x => x.Name).Indexed().Select(x => (x.Key + 1 + ". " + x.Value)).Join("\n");
                        Console.WriteLine();
                        Console.WriteLine(decksShow);
                        Console.WriteLine("\n以上是已有卡组,请输入指定卡组的序号进行导出:");
                        try
                        {
                            var c = Console.ReadLine();
                            var index = int.Parse(c);
                            if (!(index >= 1 && index <= user.Decks.Count))
                            {
                                Console.WriteLine("请重新选择正确的卡组序号!(按下回车返回主菜单)");
                                Console.ReadLine();
                                break;
                            }
                            if (!user.Decks[index - 1].IsBasicDeck())
                            {
                                Console.WriteLine("该卡组无法用于对战,请选择完整卡组!(按下回车返回主菜单)");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"卡组码:\n{user.Decks[index - 1].CompressDeck()}");
                                Console.WriteLine("请手动复制卡组码进行分享,按下回车将返回主菜单~");
                                Console.ReadLine();
                                break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("发生错误,按下回车返回主菜单");
                            Console.ReadLine();
                            break;
                        }
                    default:
                        await GwentTest.ConfirmExit();
                        return;
                }
            }

            // await GwentTest.ConfirmExit();
        }
    }
}
