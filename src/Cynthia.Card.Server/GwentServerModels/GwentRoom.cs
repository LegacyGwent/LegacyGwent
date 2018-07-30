using System;

namespace Cynthia.Card.Server
{
    public class GwentRoom
    {
        public GwentClientPlayer Player1 { get; set; }
        public GwentClientPlayer Player2 { get; set; }
        public bool IsReady { get => Player2 != null && Player1 != null; }

        public GwentRoom(GwentClientPlayer player)
        {
            Player1 = player;
        }
        public bool AddPlayer(GwentClientPlayer player)
        {
            if (Player1 == null)
            {
                Player1 = player;
                return true;
            }
            if (Player2 == null)
            {
                Player2 = player;
                return true;
            }
            return false;
        }
    }
}