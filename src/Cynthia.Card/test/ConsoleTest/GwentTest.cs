using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using CompressTest;
using Cynthia.Card;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
using Microsoft.AspNetCore.SignalR.Client;
using MongoDB.Driver;

namespace ConsoleTest
{
    public static class GwentTest
    {
        public static async Task AIBattle()
        {
            var game = new GwentServerGame(new GeraltNovaAI(), new ReaverHunterAI());

            Console.WriteLine("游戏开始~请稍等");
            var gameResult = await game.Play();

            Console.WriteLine("游戏结束,比分如下:\n");
            Console.WriteLine($"{gameResult.BluePlayerName}:\t {gameResult.BlueScore.Join(",")}");
            Console.WriteLine($"{gameResult.RedPlayerName}:\t {gameResult.RedScore.Join(",")}");
            Console.WriteLine($"胜利者为:{(gameResult.BlueWinCount > gameResult.BlueWinCount ? gameResult.BluePlayerName : gameResult.RedPlayerName)}");
        }
        public static void QueryEnvironment(DateTime time = default(DateTime), bool isAI = false)
        {
            var db = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy");
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => isDefault || x.Time >= time).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            var player = result
                .Where(x => x.IsEffective())
                .SelectMany(x => new[] { (x.BlueLeaderId, x.BluePlayerStatus()), (x.RedLeaderId, x.RedPlayerStatus()) })
                .GroupBy(x => x.Item1)
                .Select(x => new { Count = x.Count(), WinCount = x.Count(x => x.Item2 == GameStatus.Win), LoseCount = x.Count(x => x.Item2 == GameStatus.Lose), DrawCount = x.Count(x => x.Item2 == GameStatus.Draw), LeaderId = x.Key })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine($"本数据为:{(isAI ? "PVE" : "PVP")}环境");
            Console.WriteLine($"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计领袖:{player.Count}名\n其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n");
            foreach (var item in player)
            {
                Console.WriteLine($"场数:{item.Count}  胜:{item.WinCount}  负:{item.LoseCount}  平：{item.DrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)} 领袖:{GwentMap.CardMap[item.LeaderId].Name}");
            }
        }

        public static void QueryMatches(DateTime time = default(DateTime), bool isAI = false, bool isHasCode = false, bool isSuccessively = false)
        {
            var db = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy");
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => (isDefault || x.Time >= time) && (!isHasCode || (x.BlueDeckCode != null && x.RedDeckCode != null))).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            var player = result
                .Where(x => x.IsEffective())
                .SelectMany(x => new[] { (x.BluePlayerName, false, x.BluePlayerStatus()), (x.RedPlayerName, true, x.RedPlayerStatus()) })
                .GroupBy(x => x.Item1)
                .Select(x => new
                {
                    Count = x.Count(),
                    FirstCount = x.Count(x => x.Item2),
                    SecondCount = x.Count(x => !x.Item2),
                    WinCount = x.Count(x => x.Item3 == GameStatus.Win),
                    LoseCount = x.Count(x => x.Item3 == GameStatus.Lose),
                    DrawCount = x.Count(x => x.Item3 == GameStatus.Draw),
                    FirstWinCount = x.Count(x => x.Item3 == GameStatus.Win && x.Item2),
                    FirstLoseCount = x.Count(x => x.Item3 == GameStatus.Lose && x.Item2),
                    FirstDrawCount = x.Count(x => x.Item3 == GameStatus.Draw && x.Item2),
                    SecondWinCount = x.Count(x => x.Item3 == GameStatus.Win && !x.Item2),
                    SecondLoseCount = x.Count(x => x.Item3 == GameStatus.Lose && !x.Item2),
                    SecondDrawCount = x.Count(x => x.Item3 == GameStatus.Draw && !x.Item2),
                    PlayerName = x.Key
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine($"本数据为:{(isAI ? "PVE" : "PVP")}环境 {(isHasCode ? ",本数据仅包含已记录卡组码的对局。" : "")}");
            Console.WriteLine($"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计玩家:{player.Count}名");
            Console.WriteLine($"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n");
            foreach (var item in player.OrderByDescending(x => x.Count))
            {
                Console.WriteLine($"场数:{item.Count}/{item.FirstCount}/{item.SecondCount}  胜:{item.WinCount}/{item.FirstWinCount}/{item.SecondWinCount}  负:{item.LoseCount}/{item.FirstLoseCount}/{item.SecondLoseCount}  平：{item.DrawCount}/{item.FirstDrawCount}/{item.SecondDrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)}/{Math.Round(((double)item.FirstWinCount) / ((double)item.FirstCount) * 100, 2)}/{Math.Round(((double)item.SecondWinCount) / ((double)item.SecondCount) * 100, 2)} 玩家:{item.PlayerName}");
            }
        }

        public static void QueryCard(DateTime time = default(DateTime), bool isAI = false)
        {
            var db = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy");
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => (isDefault || x.Time >= time) && (x.BlueDeckCode != null && x.RedDeckCode != null)).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            // var ww = result.Where(x => x.RedDeckCode.Contains("L") || x.BlueDeckCode.Contains("$L"));

            // foreach (var item in ww)
            // {
            //     Console.WriteLine($"{item.RedDeckCode},{item.RedDeckName},{item.RedPlayerName}|{item.BlueDeckCode},{item.BlueDeckName},{item.BluePlayerName}");
            // }
            var cards = result
                .Where(x => x.IsEffective())
                .SelectMany(x => x.BlueDeckCode.DeCompressToDeck().Deck.Append(x.BlueLeaderId).Distinct().Select(card => (card, x.BluePlayerStatus()))
                    .Concat(x.RedDeckCode.DeCompressToDeck().Deck.Append(x.RedLeaderId).Distinct().Select(card => (card, x.RedPlayerStatus()))))
                // .SelectMany(x => new[] { (x.BluePlayerName, x.BluePlayerStatus()), (x.RedPlayerName, x.RedPlayerStatus()) })
                .GroupBy(x => x.Item1)
                .Select(x => new { Count = x.Count(), WinCount = x.Count(x => x.Item2 == GameStatus.Win), LoseCount = x.Count(x => x.Item2 == GameStatus.Lose), DrawCount = x.Count(x => x.Item2 == GameStatus.Draw), Card = GwentMap.CardMap[x.Key] })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine($"本数据为:{(isAI ? "PVE" : "PVP")}环境");
            Console.WriteLine($"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计使用卡牌:{cards.Count}种");
            Console.WriteLine($"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n");
            foreach (var item in cards.GroupBy(x => x.Card.Faction).OrderBy(x => x.Key))//OrderBy(x => x.Card.Faction).ThenBy(x => x.Card.Group).ThenByDescending(x => x.WinCount))
            {
                Console.WriteLine($"{GwentMap.FactionInfoMap[item.Key]}");
                foreach (var item2 in item.GroupBy(x => x.Card.Group).OrderByDescending(x => x.Key))
                {
                    Console.WriteLine($"\n{GwentMap.GroupInfoMap[item2.Key]}");
                    foreach (var card in item2.OrderByDescending(x => x.WinCount))
                    {
                        Console.WriteLine($"场数:{$"{card.Count}".PadRight(3)}  胜:{$"{card.WinCount}".PadRight(3)}  负:{$"{card.LoseCount}".PadRight(3)}  平：{$"{card.DrawCount}".PadRight(3)} 胜率:{$"{Math.Round(((double)card.WinCount) / ((double)card.Count) * 100, 2)}".PadRight(5)} 卡牌名:{card.Card.Name}");
                    }
                }
                Console.WriteLine("".PadLeft(100, '-'));
            }

            var noCard = GwentMap.CardMap.Select(x => x.Key).Except(cards.Select(x => x.Card.CardId)).Select(x => GwentMap.CardMap[x]).Where(x => !x.IsDerive);
            Console.WriteLine("\n\n以下卡牌是该时间段未被使用的卡牌\n");
            foreach (var item in noCard.GroupBy(x => x.Faction).OrderBy(x => x.Key))//OrderBy(x => x.Card.Faction).ThenBy(x => x.Card.Group).ThenByDescending(x => x.WinCount))
            {
                Console.WriteLine($"{GwentMap.FactionInfoMap[item.Key]}");
                foreach (var item2 in item.GroupBy(x => x.Group).OrderByDescending(x => x.Key))
                {
                    Console.WriteLine($"\n{GwentMap.GroupInfoMap[item2.Key]}");
                    foreach (var card in item2)
                    {
                        Console.WriteLine($"卡牌名:{card.Name}");
                    }
                }
                Console.WriteLine("".PadLeft(100, '-'));
            }
        }

        public static void QueryRanking(DateTime time = default(DateTime), bool isAI = false)
        {
            var db = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy");
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => isDefault || x.Time >= time).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            var player = result
                .Where(x => x.IsEffective())
                .SelectMany(x => new[] { (x.BluePlayerName + "/" + x.BlueDeckName, x.BluePlayerStatus()), (x.RedPlayerName + "/" + x.RedDeckName, x.RedPlayerStatus()) })
                .GroupBy(x => x.Item1)
                .Select(x => new { Count = x.Count(), WinCount = x.Count(x => x.Item2 == GameStatus.Win), LoseCount = x.Count(x => x.Item2 == GameStatus.Lose), DrawCount = x.Count(x => x.Item2 == GameStatus.Draw), PlayerName = x.Key })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine($"本数据为:{(isAI ? "PVE" : "PVP")}环境");
            Console.WriteLine($"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计玩家:{player.Count}名");
            Console.WriteLine($"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n");
            foreach (var item in player.OrderByDescending(x => x.WinCount))
            {
                Console.WriteLine($"场数:{item.Count}  胜:{item.WinCount}  负:{item.LoseCount}  平：{item.DrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)} 玩家:{item.PlayerName}");
            }
        }

        public static IMongoCollection<GameResult> GetLocalGameResultCollection()
        {
            return new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy").GetCollection<GameResult>("gameresults");
        }

        public static async Task ConfirmExit()
        {
            await Task.CompletedTask;
            Console.WriteLine("\n按回车退出");
            Console.ReadLine();
        }

        public static GameStatus BluePlayerStatus(this GameResult gameResult)
        {
            if (gameResult.BlueWinCount > gameResult.RedWinCount)
            {
                return GameStatus.Win;
            }
            if (gameResult.BlueWinCount < gameResult.RedWinCount)
            {
                return GameStatus.Lose;
            }
            return GameStatus.Draw;
        }

        public static bool IsEffective(this GameResult gameResult)
        {
            if (gameResult.RedWinCount != 2 && gameResult.BlueWinCount != 2)
            {
                return false;
            }
            return true;
        }

        public static GameStatus RedPlayerStatus(this GameResult gameResult)
        {
            if (gameResult.BlueWinCount < gameResult.RedWinCount)
            {
                return GameStatus.Win;
            }
            if (gameResult.BlueWinCount > gameResult.RedWinCount)
            {
                return GameStatus.Lose;
            }
            return GameStatus.Draw;
        }

        public static string TestCompressDeck(this DeckModel deck)
        {
            return CompressIdList(GwentMap.CardIdMap[deck.Leader], deck.Deck.Select(x => GwentMap.CardIdMap[x])).Encode();
        }

        public static byte[] CompressIdList(int leader, IEnumerable<int> list)
        {
            var query =
            from item in list
            group item by item into counting
            select (count: counting.Count(), key: counting.Key) into counted
            group counted by counted.count into gourping
            orderby gourping.Key ascending
            select (count: gourping.Key, values: (from v in gourping select v.key).ToList());

            var data = query.ToList();

            var length =
                ByteSerializer.GetFlexibleLength(leader) +
                ByteSerializer.GetFlexibleLength(data, item =>
                    ByteSerializer.GetFlexibleLength(item.count) +
                    ByteSerializer.GetFlexibleLength(item.values));

            var result = new byte[(int)Math.Ceiling(length / 8d)];
            new BitSpan(result)
                .WriteFlexible(leader)
                .WriteFlexible(data, (BitSpan span, in (int count, List<int> values) item) => span
                    .WriteFlexible(item.count)
                    .WriteFlexible(item.values));

            return result;
        }

        public static void ShowDeck(this DeckModel deck)
        {
            Console.WriteLine($"卡组:{deck.Name}");
            Console.WriteLine($"领袖:{GwentMap.CardMap[deck.Leader].Name}");
            Console.WriteLine($"卡组数量{deck.Deck.Count()}");
            var showDeck = deck.Deck.GroupBy(x => x)
                .Select(x => new { Group = GwentMap.CardMap[x.Key].Group, Count = x.Count(), Name = GwentMap.CardMap[x.Key].Name })
                .OrderByDescending(x => x.Group)
                .ThenBy(x => x.Count)
                .GroupBy(x => x.Group);
            foreach (var item in showDeck)
            {
                Console.WriteLine($"\n{item.Key}");
                foreach (var card in item)
                {
                    Console.WriteLine($"{card.Name}*{card.Count}");
                }
            }
            Console.WriteLine();
            var cd = deck.CompressDeck();
            Console.WriteLine($"卡组码:{cd}");
            Console.WriteLine($"卡组码长度:{cd.Length}");
        }

        public static async Task DeckCodeToolsAsync()
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
        }

    }
}