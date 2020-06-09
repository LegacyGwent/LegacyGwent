using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using Cynthia.Card;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
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

        public static void QueryMatches(DateTime time = default(DateTime), bool isAI = false)
        {
            var db = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy");
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => isDefault || x.Time >= time).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            var player = result
                .Where(x => x.IsEffective())
                .SelectMany(x => new[] { (x.BluePlayerName, x.BluePlayerStatus()), (x.RedPlayerName, x.RedPlayerStatus()) })
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

        public static DeckModel QueryDeck(int cardNumber = 20)
        {
            var user = new MongoClient("mongodb://localhost:27017/gwent-diy").GetDatabase("gwentdiy").GetCollection<UserInfo>("user").AsQueryable();

            return user.SelectMany(x => x.Decks).First(x => x.Deck.Count == cardNumber);
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

        public static string ZipCompressDeck(this DeckModel deck)
        {
            var list = new List<ushort>() { GwentMap.CardIdMap[deck.Leader] };
            list.AddRange(deck.Deck.Select(x => GwentMap.CardIdMap[x]).OrderBy(x => x));
            return list.ToArray().AsSpan().Strip(10).Encode();//.AsSpan().Encode();
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
    }
}