using System.Collections.Generic;
using Cynthia.Card.Common.Models;
using System;
using System.Linq;


namespace Cynthia.Card.Common.Models
{
    public class Season
    {
        public int id;
        public string name;
        public string color;
        public DateTime endTime;
        public List<SeasonReward> seasonalRewards { get; set; }

        static public string EncodeRanklist(IList<Tuple<string, string, string, string, int, int, IList<int[]>>> players)
        {
            var encoded = players.Select(p =>
            {
                string streakEncoded = string.Join(";", p.Item7.Select(arr =>
                    arr.Length == 3
                        ? $"{arr[0]},{arr[1]},{arr[2]}"
                        : throw new InvalidOperationException("Expected int[3] in streak.")));

                return $"{p.Item1}|{p.Item2}|{p.Item3}|{p.Item4}|{p.Item5}|{p.Item6}|{streakEncoded}";
            });

            return string.Join("\n", encoded);
        }

        static public IList<Tuple<string, string, string, string, int, int, IList<int[]>>> DecodeRanklist(string codedRankList)
        {
            var lines = codedRankList.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<Tuple<string, string, string, string, int, int, IList<int[]>>>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length != 7)
                    return result;

                string playerName = parts[0];
                string avatar = parts[1];
                string border = parts[2];
                string title = parts[3];
                int mmr = int.Parse(parts[4]);
                int highestMMR = int.Parse(parts[5]);

                // Dekoduj streak: "1,2,3;4,5,6;..."
                var streakParts = parts[6].Split(';');
                var streakList = new List<int[]>();

                foreach (var streak in streakParts)
                {
                    var numbers = streak.Split(',').Select(s => int.Parse(s)).ToArray();
                    if (numbers.Length != 3)
                        return result;
                    streakList.Add(numbers);
                }

                result.Add(Tuple.Create(playerName, avatar, border, title, mmr, highestMMR, (IList<int[]>)streakList));
            }

            return result;
        }

        static public int CalculateFactionPoints(float gamesWon, float gamesLost, float gamesDrawn)
        {
            var faction_games = gamesWon + gamesLost + gamesDrawn;
            if (faction_games == 0)
                return 0;
            var a = 0.0f;
            var b = 4000.0f;
            var ewr = (gamesWon + 0.5 * gamesDrawn + a) / (gamesWon + gamesLost + gamesDrawn + a + b) * 100;
            var ewrWeighted = ewr + Math.Log10(faction_games) * 0.02f;
            int points = (int)Math.Round(ewrWeighted * 10000);
            return points;
        }
        
        static public Dictionary<int, List<Tuple<string, int>>> CalculateFactions(IList<Tuple<string, string, string, string, int, int, IList<int[]>>> rankingList, int limit = 300)
        {
            var factionsRanking = new Dictionary<int, List<Tuple<string, int>>>();

            for (int i = 0; i < 5; i++)
            {
                factionsRanking[i] = new List<Tuple<string, int>>();
            }

            foreach (var playerData in rankingList)
            {
                var playerNickName = playerData.Item1;
                var playerFactionsStreaks = playerData.Item7;

                for (int i = 0; i < 5; i++)
                {
                    var wins = playerFactionsStreaks[i][0];
                    var loses = playerFactionsStreaks[i][1];
                    var draws = playerFactionsStreaks[i][2];
                    factionsRanking[i].Add(new Tuple<string, int>(playerNickName, Season.CalculateFactionPoints(wins, loses, draws)));
                }
            }

            for (int i = 0; i < 5; i++)
            {
                factionsRanking[i] = factionsRanking[i]
                    .OrderByDescending(x => x.Item2).Take(limit)
                    .ToList();
            }

            return factionsRanking;
        }


    }

    public class SeasonInfo : ModelBase
    {
        public object this[string propertyName] // allows the ["property"] syntax
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        public bool isActive { get; set; }
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }
        public string SeasonColor { get; set; }
        public bool areRewardsGranted { get; set; } = false;
        public List<SeasonReward> seasonalRewards { get; set; } = new List<SeasonReward>();
        public DateTime SeasonStartTime { get; set; } = DateTime.UtcNow;
        public DateTime SeasonEndTime { get; set; } = DateTime.UtcNow;
        public string rankingHistory { get; set; } = "";

    }
}
