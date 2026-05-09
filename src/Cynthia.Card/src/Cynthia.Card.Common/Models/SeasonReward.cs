namespace Cynthia.Card.Common.Models
{
    public class SeasonReward
    {
        public int minimalPosition { get; set; }
        public string avatar { get; set; }
        public string border { get; set; }
        public string title { get; set; }

        // Indicates that this reward is granted during the season
        // when certain conditions (e.g. MMR threshold) are met,
        // instead of at season end based on leaderboard position.
        public bool isInSeasonReward { get; set; } = false;

        // Minimal MMR a player has to reach in the current season
        // to be eligible for this in-season reward. Ignored for
        // season-end (position-based) rewards.
        public int minimalMMR { get; set; } = 0;

        public SeasonReward(
            int minimalPosition,
            string avatar = null,
            string border = null,
            string title = null,
            bool isInSeasonReward = false,
            int minimalMMR = 0)
        {
            this.minimalPosition = minimalPosition;
            this.avatar = avatar;
            this.border = border;
            this.title = title;
            this.isInSeasonReward = isInSeasonReward;
            this.minimalMMR = minimalMMR;
        }
    }
} 