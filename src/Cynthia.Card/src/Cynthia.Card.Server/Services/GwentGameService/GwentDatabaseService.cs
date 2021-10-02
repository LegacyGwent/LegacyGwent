using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Cynthia.Card.Server
{
    public class GwentDatabaseService
    {
        private readonly IServiceProvider _provider;
        // public IDatabaseService Database { get; set; }
        // private IRepository<UserInfo> _collection;
        public IMongoClient GetMongoClient()
        {
            var result = (IMongoClient)_provider.GetService(typeof(IMongoClient));
            return result;
        }
        private IMongoDatabase GetDatabase() => GetMongoClient().GetDatabase(_dataBaseName);
        private IMongoCollection<UserInfo> GetUserInfo() => GetDatabase().GetCollection<UserInfo>(_repositoryName);

        private const string _dataBaseName = "gwentdiy";
        private const string _repositoryName = "user";
        public GwentDatabaseService(IServiceProvider provider)
        {
            _provider = provider;
            // Database = database;
            // _collection = Database[_dataBaseName].GetRepository<UserInfo>(_repositoryName);
        }
        public bool AddDeck(string username, DeckModel deck)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            //var user = _collection.AsQueryable().Single(x => x.UserName == username);
            if (user.Decks.Any(x => x.Id == deck.Id))
                return false;
            user.Decks.Add(deck);
            temp.ReplaceOne(x => x.UserName == username, user);
            //_collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool ModifyDeck(string username, string id, DeckModel deck)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            user.Decks[user.Decks.Select((x, index) => (x, index)).Single(d => d.x.Id == id).index] = deck;
            temp.ReplaceOne(x => x.UserName == username, user);
            return true;
        }
        public bool ModifyBlacklist(string username, BlacklistModel blacklist)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            user.Blacklist = blacklist;
            temp.ReplaceOne(x => x.UserName == username, user);
            return true;
        }
        public bool RemoveDeck(string username, string id)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            user.Decks.RemoveAt(user.Decks.Select((x, index) => (x, index)).Single(deck => deck.x.Id == id).index);
            temp.ReplaceOne(x => x.UserName == username, user);
            // _collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool Register(string username, string password, string playername)
        {
            var temp = GetUserInfo();
            if (temp.AsQueryable<UserInfo>().Any(x => x.UserName == username || x.PlayerName == playername))
            {
                return false;
            }
            var decks = new List<DeckModel>();
            decks.Add(GwentDeck.CreateBasicDeck(1));
            temp.InsertOne(new UserInfo { UserName = username, PassWord = password, PlayerName = playername, Decks = decks });
            return true;
        }
        public UserInfo Login(string username, string password)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable<UserInfo>().Where(x => x.UserName == username && x.PassWord == password).ToArray();
            return user.Length > 0 ? user[0] : null;
        }

        public IList<GameResult> GetAllGameResults(int count)
        {
            var temp = GetDatabase().GetCollection<GameResult>("gameresults");
            return temp.AsQueryable<GameResult>().OrderByDescending(x => x.Time).Take(count).ToList();
        }
        public bool AddGameResult(GameResult data)
        {
            var temp = GetDatabase().GetCollection<GameResult>("gameresults");
            if (temp.AsQueryable().Any(x => data.Id == x.Id))
            {
                return false;
            }
            temp.InsertOne(data);
            return true;
        }

        public bool AddAIGameResult(GameResult data)
        {
            var temp = GetDatabase().GetCollection<GameResult>("aigameresults");
            if (temp.AsQueryable().Any(x => data.Id == x.Id))
            {
                return false;
            }
            temp.InsertOne(data);
            return true;
        }

        public IEnumerable<GameResult> GetGameResults(DateTime time)
        {
            var db = GetDatabase();
            var gameResult = db.GetCollection<GameResult>("gameresults");
            return gameResult.AsQueryable().Where(x => x.Time >= time).ToList().Select(x => { x.BlueDeckCode = ""; x.RedDeckCode = ""; return x; }).ToList();
        }

        public string QueryEnvironment(DateTime time)
        {
            var isAI = false;
            var db = GetDatabase();
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

            var str = "";

            str += $"本数据为:{(isAI ? "PVE" : "PVP")}环境\n";
            str += $"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计领袖:{player.Count}名\n其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n\n";
            foreach (var item in player)
            {
                str += $"场数:{item.Count}  胜:{item.WinCount}  负:{item.LoseCount}  平：{item.DrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)} 领袖:{GwentMap.CardMap[item.LeaderId].Name}\n";
            }

            return str;
        }

        public string QueryMatches(DateTime time)
        {
            var isAI = false;
            var isHasCode = false;
            var db = GetDatabase();
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

            var str = "";

            str += $"本数据为:{(isAI ? "PVE" : "PVP")}环境 {(isHasCode ? ",本数据仅包含已记录卡组码的对局。" : "")}\n";
            str += $"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计玩家:{player.Count}名\n";
            str += $"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n\n";
            foreach (var item in player.OrderByDescending(x => x.Count))
            {
                str += $"场数:{item.Count}/{item.FirstCount}/{item.SecondCount}  胜:{item.WinCount}/{item.FirstWinCount}/{item.SecondWinCount}  负:{item.LoseCount}/{item.FirstLoseCount}/{item.SecondLoseCount}  平：{item.DrawCount}/{item.FirstDrawCount}/{item.SecondDrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)}/{Math.Round(((double)item.FirstWinCount) / ((double)item.FirstCount) * 100, 2)}/{Math.Round(((double)item.SecondWinCount) / ((double)item.SecondCount) * 100, 2)} 玩家:{item.PlayerName}\n";
            }

            return str;
        }

        public string QueryCard(DateTime time)
        {
            var isAI = false;
            var db = GetDatabase();
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            var result = resultCollection.AsQueryable().Where(x => (isDefault || x.Time >= time) && (x.BlueDeckCode != null && x.RedDeckCode != null)).ToList();
            var badCount = result.Where(x => !x.IsEffective()).Count();
            var count = result.Count();

            var cards = result
                .Where(x => x.IsEffective())
                .SelectMany(x => x.BlueDeckCode.DeCompressToDeck().Deck.Append(x.BlueLeaderId).Distinct().Select(card => (card, x.BluePlayerStatus()))
                    .Concat(x.RedDeckCode.DeCompressToDeck().Deck.Append(x.RedLeaderId).Distinct().Select(card => (card, x.RedPlayerStatus()))))
                // .SelectMany(x => new[] { (x.BluePlayerName, x.BluePlayerStatus()), (x.RedPlayerName, x.RedPlayerStatus()) })
                .GroupBy(x => x.Item1)
                .Select(x => new { Count = x.Count(), WinCount = x.Count(x => x.Item2 == GameStatus.Win), LoseCount = x.Count(x => x.Item2 == GameStatus.Lose), DrawCount = x.Count(x => x.Item2 == GameStatus.Draw), Card = GwentMap.CardMap[x.Key] })
                .OrderByDescending(x => x.Count)
                .ToList();

            var str = "";

            str += $"本数据为:{(isAI ? "PVE" : "PVP")}环境\n";
            str += $"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计使用卡牌:{cards.Count}种\n";
            str += $"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n\n";
            foreach (var item in cards.GroupBy(x => x.Card.Faction).OrderBy(x => x.Key))//OrderBy(x => x.Card.Faction).ThenBy(x => x.Card.Group).ThenByDescending(x => x.WinCount))
            {
                str += $"{GwentMap.FactionInfoMap[item.Key]}\n";
                foreach (var item2 in item.GroupBy(x => x.Card.Group).OrderByDescending(x => x.Key))
                {
                    str += $"\n{GwentMap.GroupInfoMap[item2.Key]}\n";
                    foreach (var card in item2.OrderByDescending(x => x.WinCount))
                    {
                        str += $"场数:{$"{card.Count}".PadRight(3)}  胜:{$"{card.WinCount}".PadRight(3)}  负:{$"{card.LoseCount}".PadRight(3)}  平：{$"{card.DrawCount}".PadRight(3)} 胜率:{$"{Math.Round(((double)card.WinCount) / ((double)card.Count) * 100, 2)}".PadRight(5)} 卡牌名:{card.Card.Name}\n";
                    }
                }
                str += "".PadLeft(100, '-') + "\n";
            }

            var noCard = GwentMap.CardMap.Select(x => x.Key).Except(cards.Select(x => x.Card.CardId)).Select(x => GwentMap.CardMap[x]).Where(x => !x.IsDerive);
            str += "\n\n以下卡牌是该时间段未被使用的卡牌\n\n";
            foreach (var item in noCard.GroupBy(x => x.Faction).OrderBy(x => x.Key))//OrderBy(x => x.Card.Faction).ThenBy(x => x.Card.Group).ThenByDescending(x => x.WinCount))
            {
                str += $"{GwentMap.FactionInfoMap[item.Key]}\n";
                foreach (var item2 in item.GroupBy(x => x.Group).OrderByDescending(x => x.Key))
                {
                    str += $"\n{GwentMap.GroupInfoMap[item2.Key]}\n";
                    foreach (var card in item2)
                    {
                        str += $"卡牌名:{card.Name}\n";
                    }
                }
                str += "".PadLeft(100, '-') + "\n";
            }

            return str;
        }

        public string QueryRanking(DateTime time)
        {
            var isAI = false;
            var db = GetDatabase();
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

            var str = "";

            str += $"本数据为:{(isAI ? "PVE" : "PVP")}环境\n";
            str += $"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计玩家:{player.Count}名\n";
            str += $"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n\n";
            foreach (var item in player.OrderByDescending(x => x.WinCount))
            {
                str += $"场数:{item.Count}  胜:{item.WinCount}  负:{item.LoseCount}  平：{item.DrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)} 玩家:{item.PlayerName}\n";
            }

            return str;
        }
    }
}