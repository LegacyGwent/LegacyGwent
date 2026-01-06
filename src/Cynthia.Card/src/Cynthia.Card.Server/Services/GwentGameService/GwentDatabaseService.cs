using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Cynthia.Card;
using Cynthia.Card.Common.Models;
using System.Threading.Tasks;










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

        public IMongoCollection<SeasonInfo> GetSeasonInfo() => GetDatabase().GetCollection<SeasonInfo>(_seasonRepositoryName);

        private const string _dataBaseName = "gwentdiy";
        private const string _repositoryName = "user";

        private const string _seasonRepositoryName = "season";
        public GwentDatabaseService(IServiceProvider provider)
        {
            _provider = provider;
            // Database = database;
            // _collection = Database[_dataBaseName].GetRepository<UserInfo>(_repositoryName);
        }

        public async Task UpdateSeasons(List<Season> seasonsList)
        {
            var temp = GetSeasonInfo();
            foreach (var season in seasonsList)
            {
                var filter = Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, season.id);
                bool isIdPresent = await temp.Find(filter).AnyAsync();

                if (!isIdPresent)
                {
                    await temp.InsertOneAsync(new SeasonInfo
                    {
                        isActive = false,
                        SeasonId = season.id,
                        SeasonName = season.name,
                        SeasonColor = season.color,
                        SeasonEndTime = season.endTime,
                        seasonalRewards = season.seasonalRewards
                    });
                }
                else
                {
                    var update = Builders<SeasonInfo>.Update
                        .Set(x => x.SeasonName, season.name)
                        .Set(x => x.SeasonEndTime, season.endTime)
                        .Set(x => x.seasonalRewards, season.seasonalRewards);

                    await temp.UpdateOneAsync(filter, update);
                }
            }
        }

        public async Task RefreshSeasons()
        {
            string[] SeasonsColorsList = {"darkyellow", "emerald", "orange", "lightblue", "blue", "yellow", "lightgreen", "red", "nrblue", "darkgreen"};
            var temp = GetSeasonInfo();
            var filter = Builders<SeasonInfo>.Filter.Eq(x => x.isActive, true);
            var sortById = Builders<SeasonInfo>.Sort.Ascending(x => x.SeasonId);

            SeasonInfo nextSeason = null;
            TimeSpan _autoSeasonDuration = TimeSpan.FromDays(60);

            try
            {
                var all_seasons = await temp.Find(_ => true).Sort(sortById).ToListAsync();
                var last_index = all_seasons.Last().SeasonId;

                var firstActiveSeason = await temp.Find(filter).Sort(sortById).FirstOrDefaultAsync();


                if (firstActiveSeason != null)
                {
                    var filterNextInactive = Builders<SeasonInfo>.Filter.And(
                        Builders<SeasonInfo>.Filter.Eq(x => x.isActive, false),
                        Builders<SeasonInfo>.Filter.Gt(x => x.SeasonId, firstActiveSeason.SeasonId)
                    );

                    var nextInactiveSeason = await temp.Find(filterNextInactive).Sort(sortById).FirstOrDefaultAsync();

                    if (nextInactiveSeason != null)
                    {
                        nextSeason = nextInactiveSeason;
                    }

                    //NO SEASON WITH BIGGER INDEX
                    else
                    {
                        /*nextSeason = new SeasonInfo
                        {
                            isActive = false,
                            SeasonId = last_index + 1,
                            SeasonName = "Season_CreaturesSeason",
                            SeasonColor = SeasonsColorsList[new Random().Next(SeasonsColorsList.Length)],
                            SeasonStartTime = DateTime.UtcNow,
                            SeasonEndTime = DateTime.UtcNow + _autoSeasonDuration,
                            seasonalRewards = firstActiveSeason.seasonalRewards
                        };
                        await temp.InsertOneAsync(nextSeason);*/
                    }
                    //SET THE OLD TO INACTIVE IF ITS DATE PAST
                    if (firstActiveSeason.areRewardsGranted && DateTime.UtcNow > firstActiveSeason.SeasonEndTime)
                    {
                        var OldSeasonFilter = Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, firstActiveSeason.SeasonId);
                        var OldSeasonUpdate = Builders<SeasonInfo>.Update.Set(x => x.isActive, false);
                        firstActiveSeason.isActive = false;

                        await GetSeasonInfo().UpdateOneAsync(OldSeasonFilter, OldSeasonUpdate);
                    }

                }

                if (firstActiveSeason == null || !firstActiveSeason.isActive) //Means None is active at the moment
                {
                    int baseSeasonId = firstActiveSeason?.SeasonId ?? 0;
                    var seasons = await temp.Find(s => s.SeasonId > baseSeasonId).Sort(sortById).ToListAsync();
                    
                    if (seasons.Count > 0)
                    {
                        nextSeason = seasons.First();
                    }
                    else
                    {
                        var seasonsWithRewardsSeted = await temp.Find(s => s.seasonalRewards.Count > 0).Sort(sortById).ToListAsync();
                        var lastElementWithRewardsSeted = seasonsWithRewardsSeted.LastOrDefault();
                        nextSeason = new SeasonInfo
                        {
                            isActive = false,
                            SeasonId = last_index + 1,
                            SeasonName = "Season_CreaturesSeason",
                            SeasonColor = SeasonsColorsList[new Random().Next(SeasonsColorsList.Length)],
                            SeasonStartTime = DateTime.UtcNow,
                            SeasonEndTime = DateTime.UtcNow + _autoSeasonDuration,
                            seasonalRewards = lastElementWithRewardsSeted.seasonalRewards
                        };
                        await temp.InsertOneAsync(nextSeason);
                    }

                    if (nextSeason != null)
                    {
                        var filterSameId = Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, nextSeason.SeasonId);
                        var update = Builders<SeasonInfo>.Update
                            .Set(x => x.isActive, true)
                            .Set(x => x.SeasonStartTime, DateTime.UtcNow);
                        await temp.UpdateOneAsync(filterSameId, update);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public void SeasonRewardsGranted()
        {
            var temp = GetSeasonInfo();
            var activeSeason = temp.AsQueryable().Where(x => x.isActive == true).OrderByDescending(x => x.SeasonId).FirstOrDefault();

            if (activeSeason != null)
            {
                var filter = Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, activeSeason.SeasonId);
                var update = Builders<SeasonInfo>.Update.Set(x => x.areRewardsGranted, true);
                var result = temp.UpdateOne(filter, update);
            }
        }

        public List<SeasonReward> QuerySeasonRewards(int seasonID, string type)
        {
            SeasonInfo season;
            if (seasonID < 0)
                season = GetSeasonInfo().AsQueryable().Where(x => x.isActive).OrderByDescending(x => x.SeasonId).FirstOrDefault();
            else
                season = GetSeasonInfo().AsQueryable().Where(x => x.SeasonId == seasonID).FirstOrDefault();
            if (season == null)
                return new List<SeasonReward>();
            return season.seasonalRewards;

        }
        public async Task<SeasonInfo> QuerySeasonData(bool active = true, int id = 0)
        {
            var seasons = GetSeasonInfo().Find(_ => true).ToList();

            if (active)
            {
                var activeSeason = seasons
                    .Where(x => x.isActive)
                    .OrderBy(x => x.SeasonId)
                    .FirstOrDefault();

                return activeSeason ?? new SeasonInfo();
            }
            else
            {
                var searchedSeason = seasons
                    .FirstOrDefault(x => x.SeasonId == id);

                return searchedSeason ?? new SeasonInfo(){SeasonId = -1};
            }
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
        public int initMMR = 3400;
        public bool Register(string username, string password, string playername)
        {
            var temp = GetUserInfo();
            if (temp.AsQueryable<UserInfo>().Any(x => x.UserName == username || x.PlayerName == playername))
            {
                return false;
            }
            var decks = new List<DeckModel>();
            var ownedavatars = new List<string>();
            ownedavatars.Add("GeraltOfRivia");
            var ownedborders = new List<string>();
            ownedborders.Add("NoBorder");
            var ownedtitles = new List<string>();
            ownedtitles.Add("NoBorder");
            decks.Add(GwentDeck.CreateBasicDeck(1));

            var emptyStreak = new List<int[]>() { new int[3], new int[3], new int[3], new int[3], new int[3] };

            temp.InsertOne(new UserInfo { UserName = username, PassWord = password, PlayerName = playername, Decks = decks, MMR = initMMR, HighestMMR = initMMR, OwnedAvatars = ownedavatars, OwnedBorders = ownedborders });
            return true;
        }
        public UserInfo Login(string username, string password)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable<UserInfo>().Where(x => x.UserName == username && x.PassWord == password).ToArray();

            if (user.Length <= 0)
            {
                return null;
            }

            return user[0];
        }
        public bool UpdateMMR(string playername, int MMR)//更新玩家天梯分数
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].MMR = MMR;
            if (user[0].MMR > user[0].HighestMMR)
            {
                user[0].HighestMMR = MMR;
            }
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        // add an avatar to the user's owned avatars
        public bool AddAvatar(string playername, string AvatarID)
        {
            //check if AvatarID exists before adding
            if (!TrinketMap.GetAvatarsId().Any(x => x == AvatarID)) { return false; }
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user[0].OwnedAvatars == null)
            {
                user[0].OwnedAvatars = new List<string>();
            }
            if (!user[0].OwnedAvatars.Any(x => x == AvatarID))
            {
                user[0].OwnedAvatars.Add(AvatarID);
                temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
                return true;
            }
            else
            {
                return false;
            }
            
        }
        // add a border to the user's owned borders
        public bool AddBorder(string playername, string BorderID)
        {
            // check if BorderID exists before adding
            if (!TrinketMap.GetBordersId().Any(x => x == BorderID)) { return false; }
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user[0].OwnedBorders == null)
            {
                user[0].OwnedBorders = new List<string>();
            }
            if (!user[0].OwnedBorders.Any(x => x == BorderID))
            {
                user[0].OwnedBorders.Add(BorderID);
                temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool AddTitle(string playername, string TitleID)
        {
            // check if TitleID exists before adding
            if (!TrinketMap.GetTitlesId().Any(x => x == TitleID)) { return false; }
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user[0].OwnedTitles == null)
            {
                user[0].OwnedTitles = new List<string>();
            }
            if (!user[0].OwnedTitles.Any(x => x == TitleID))
            {
                user[0].OwnedTitles.Add(TitleID);
                temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool UpdateAvatar(string playername, string AvatarID) // Set the avatar of the user
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].CurrentAvatar = AvatarID;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool UpdateBorder(string playername, string BorderID) // Set the border of the user
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].CurrentBorder = BorderID;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool UpdateTitle(string playername, string TitleID) // Set the title of the user
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].CurrentTitle = TitleID;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool UpdateGGCounter(string playername) // increase the gg count of a player
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].GGsReceived += 1;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool UpdateStreak(string playername, int result, int factionIndex)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].Streak[factionIndex][result] += 1;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool SetStreak(string playername, IList<int[]> streak)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0 || streak == null || streak.Count == 0)
            {
                return false;
            }
            user[0].Streak = streak;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }
        public bool UpdateGamesOver200(string playername) // increase the games over 200 points count of a player
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            if (user.Length == 0)
            {
                return false;
            }
            user[0].GamesOver200 += 1;
            temp.ReplaceOne(x => x.PlayerName == playername, user[0]);
            return true;
        }

        public int QueryMMR(string playername)//计算玩家天梯分数
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            return user.Length > 0 ? user[0].MMR : 0;
        }
        public Tuple<int, int> QueryMMRandPeak(string playername)//计算玩家天梯分数
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            return user.Length > 0 ? new Tuple<int, int>(user[0].MMR, user[0].HighestMMR) : new Tuple<int, int>(0, 0);
        }
        public IList<string> QueryUserMessages(string playername)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();
            return user.Length > 0 ? user[0].UserMessages : new List<string>();
        }

        public int[] QueryStreak(string playername, int factionId = -1)
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.PlayerName == playername).ToArray();

            if (user.Length > 0)
            {
                if (factionId == -1)
                {
                    var totalStreak = new int[3];
                    foreach (var factionStreak in user[0].Streak)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            totalStreak[i] += factionStreak[i];
                        }
                    }
                    return totalStreak;
                }
                return user[0].Streak[factionId];
            }
            return new int[3];
        }
        public IList<Tuple<string, int>> QueryAllMMR(int offset, int limit)//所有玩家天梯分数
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.MMR != initMMR).OrderByDescending(x => x.MMR).Skip(offset).Take(limit).ToList();
            var pairs = user.Select(x => new Tuple<string, int>(x.PlayerName, x.MMR)).ToList();
            return pairs;
        }
        public IList<Tuple<string, string, string, string, int, int, IList<int[]>>> QueryAllMMRExtended(int offset, int limit)//所有玩家天梯分数
        {
            var temp = GetUserInfo();
            var users = temp.AsQueryable().OrderByDescending(x => x.MMR).Skip(offset).Take(limit).ToList();
            //var user = temp.AsQueryable().Where(x => x.MMR != initMMR).OrderByDescending(x => x.MMR).Skip(offset).Take(limit).ToList();

            var pairs = users.Select(x =>
            {
                return new Tuple<string, string, string, string, int, int, IList<int[]>>(x.PlayerName, x.CurrentAvatar, x.CurrentBorder, x.CurrentTitle, x.MMR, x.HighestMMR, x.Streak);
            }).ToList();

            return pairs;
        }

        public IList<Tuple<string, int>> QueryAllHighestMMR(int offset, int limit)//所有玩家天梯最高分数
        {
            var temp = GetUserInfo();
            var user = temp.AsQueryable().Where(x => x.HighestMMR != 0).OrderByDescending(x => x.HighestMMR).Skip(offset).Take(limit).ToList();
            var pairs = user.Select(x => new Tuple<string, int>(x.PlayerName, x.HighestMMR)).ToList();
            return pairs;
        }

        public IList<GameResult> GetAllGameResults(int count, bool rankedOnly = false)
        {
            var temp = GetDatabase().GetCollection<GameResult>("gameresults");
            if (count == -1)
            {
                if (rankedOnly)
                    return temp.AsQueryable<GameResult>().OrderByDescending(x => x.Time).Where(x => x.isRanked == true).Take(100000).ToList();
                return temp.AsQueryable<GameResult>().OrderByDescending(x => x.Time).Take(100000).ToList();
            }
            if (rankedOnly)
                return temp.AsQueryable<GameResult>().OrderByDescending(x => x.Time).Where(x => x.isRanked == true).Take(count).ToList();
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

        public string QueryCard(DateTime time, bool? rankeds)
        {
            var isAI = false;
            var db = GetDatabase();
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            List<GameResult> result;
            if (rankeds != null)
                result = resultCollection.AsQueryable().Where(x => x.isRanked == rankeds && (isDefault || x.Time >= time) && (x.BlueDeckCode != null && x.RedDeckCode != null)).ToList();
            else
                result = resultCollection.AsQueryable().Where(x => (isDefault || x.Time >= time) && (x.BlueDeckCode != null && x.RedDeckCode != null)).ToList();
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
            if (rankeds.HasValue)
                str += $"Type: {(rankeds.Value ? "Ranked" : "Casual")}\n";
            else
                str += $"(/true - only ranking games, /false - only casual)\n";

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

        public string QueryRanking(DateTime time, bool? rankeds)
        {
            var isAI = false;
            var db = GetDatabase();
            var resultCollection = isAI ? db.GetCollection<GameResult>("aigameresults") : db.GetCollection<GameResult>("gameresults");
            var isDefault = time == default(DateTime);
            List<GameResult> result;
            if (rankeds != null)
                result = resultCollection.AsQueryable().Where(x => x.isRanked == rankeds && (isDefault || x.Time >= time)).ToList();
            else
                result = resultCollection.AsQueryable().Where(x => isDefault || x.Time >= time).ToList();
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

            if (rankeds.HasValue)
                str += $"Type: {(rankeds.Value ? "Ranked" : "Casual")}\n";
            else
                str += $"(/true - only ranking games, /false - only casual)\n";

            str += $"本数据为:{(isAI ? "PVE" : "PVP")}环境\n";
            str += $"diy服{(isDefault ? "" : time + "后")}后共计对局{count}场\n共计玩家:{player.Count}名\n";
            str += $"其中无效对局{badCount}场[强退,掉线等],无效对局不计入以下统计\n\n";
            foreach (var item in player.OrderByDescending(x => x.WinCount))
            {
                str += $"场数:{item.Count}  胜:{item.WinCount}  负:{item.LoseCount}  平:{item.DrawCount} 胜率:{Math.Round(((double)item.WinCount) / ((double)item.Count) * 100, 2)} 玩家:{item.PlayerName}\n";
            }

            return str;
        }

        public IList<SeasonInfo> QuerySeasons()
        {
            return GetSeasonInfo().Find(_ => true).ToList();
        }

        public IEnumerable<UserInfo> GetAllPlayers()
        {
            return GetUserInfo().Find(_ => true).ToList();
        }

        public async Task ResetPlayerMMR(string username, int baseMMR)
        {
            var filter = Builders<UserInfo>.Filter.Eq(x => x.UserName, username);
            var update = Builders<UserInfo>.Update
                .Set(x => x.MMR, baseMMR)
                .Set(x => x.HighestMMR, baseMMR);

            await GetUserInfo().UpdateOneAsync(filter, update);
        }
        public async Task ResetPlayerStreak(string username)
        {
            var filter = Builders<UserInfo>.Filter.Eq(x => x.UserName, username);
            var update = Builders<UserInfo>.Update
                .Set(x => x.Streak, new List<int[]>(){
                                                new int[3] { 0, 0, 0 },
                                                new int[3] { 0, 0, 0 },
                                                new int[3] { 0, 0, 0 },
                                                new int[3] { 0, 0, 0 },
                                                new int[3] { 0, 0, 0 }
                                                });

            await GetUserInfo().UpdateOneAsync(filter, update);
        }

        public async Task<bool> SaveUserMessage(string username, UserMessage message)
        {
            var temp = GetUserInfo();
            var filter = Builders<UserInfo>.Filter.Eq(x => x.UserName, username);
            var user = await temp.Find(filter).FirstOrDefaultAsync();

            if (user != null)
            {
                int newMessageId;
                if (user.UserMessages == null || !user.UserMessages.Any())
                {
                    user.UserMessages = new List<string>();
                    newMessageId = 1;
                }
                else
                {
                    newMessageId = UserMessage.ReCreateMessage(user.UserMessages.Last()).MessageId + 1;
                }
                message.MessageId = newMessageId;

                if (message is UserSeasonEndMessage seasondEndMsg)
                {
                    string condensedMessage = $"UserSeasonEndMessage|{seasondEndMsg.MessageId.ToString()}|{string.Join(",", seasondEndMsg.avatars)}|{string.Join(",", seasondEndMsg.borders)}|{string.Join(",", seasondEndMsg.titles)}|{seasondEndMsg.mmrBeforeReset}|{seasondEndMsg.rank}|{seasondEndMsg.seasonName}";

                    user.UserMessages.Add(condensedMessage);
                    var update = Builders<UserInfo>.Update.Set(x => x.UserMessages, user.UserMessages);
                    await temp.UpdateOneAsync(filter, update);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveUserMessage(string username, int messageToRemoveId)
        {
            var temp = GetUserInfo();
            var filter = Builders<UserInfo>.Filter.Eq(x => x.UserName, username);
            var user = await temp.Find(filter).FirstOrDefaultAsync();
            if (user != null)
            {
                var messages = user.UserMessages;

                foreach (var codedMessage in messages)
                {
                    if (UserMessage.ReCreateMessage(codedMessage).MessageId == messageToRemoveId)
                    {
                        messages.Remove(codedMessage);
                        var update = Builders<UserInfo>.Update.Set(x => x.UserMessages, messages);
                        await temp.UpdateOneAsync(filter, update);
                        break;
                    }
                }
            }
            return false;
        }
        
    }
    
}