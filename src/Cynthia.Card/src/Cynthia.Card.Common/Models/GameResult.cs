using System;

namespace Cynthia.Card
{
    public class GameResult : ModelBase
    {
        public DateTime Time { get; set; }
        public string RedPlayerName { get; set; }
        public string BluePlayerName { get; set; }
        public string RedDeckName { get; set; }
        public string BlueDeckName { get; set; }

        public GameStatus RedPlayerGameResultStatus { get; set; }
        public string RedLeaderId { get; set; }
        public string BlueLeaderId { get; set; }

        public int RedWinCount { get; set; }
        public int BlueWinCount { get; set; }

        public int ValidCount { get; set; }
        public int[] RedScore { get; set; }
        public int[] BlueScore { get; set; }

        public string RedDeckCode { get; set; }

        public string BlueDeckCode { get; set; }
        public bool isSurrender { get; set; }
        public bool isSpecial { get; set; }
        public int BalancePoint { get; set; }
        public string RedBlacklistCode { get; set; }
        public string BlueBlacklistCode { get; set; }
    }
}