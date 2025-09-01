using System.Collections.Generic;

namespace Cynthia.Card.Server
{
    public enum TrinketType
    {
        Avatar,
        Title,
        Border
    }

    public class AwardTrinketRequest
    {
        public List<string> Usernames { get; set; }
        public string TrinketId { get; set; }
        public TrinketType TrinketType { get; set; }
    }
}
