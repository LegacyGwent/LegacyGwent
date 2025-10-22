namespace Cynthia.Card.Common.Models
{
    public class SeasonReward
    {
        public int minimalPosition { get; set; }
        public string avatar { get; set; }
        public string border { get; set; }
        public string title { get; set; }

        public SeasonReward(int minimalPosition, string avatar = null, string border = null, string title = null)
        {
            this.minimalPosition = minimalPosition;
            this.avatar = avatar;
            this.border = border;
            this.title = title;
        }
    }
} 