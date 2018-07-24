using System;

namespace Cynthia.Card.Server
{
    public class GwentRoom
    {
        public GwentServerPlayer HomePlayer { get; set; }
        public GwentServerPlayer JoinPlayer { get; set; }
        public string RoomID { get; }

        public GwentRoom(GwentServerPlayer player)
        {
            HomePlayer = player;
            RoomID = Guid.NewGuid().ToString();
        }
        public bool AddPlayer(GwentServerPlayer player)
        {
            if (HomePlayer == null && JoinPlayer == null)
            {
                HomePlayer = player;
                return false;
            }
            if (HomePlayer == null)
            {
                HomePlayer = player;
                return true;
            }
            if (JoinPlayer == null)
            {
                JoinPlayer = player;
                return true;
            }
            return false;
        }
        public bool IsReady()
        {
            return JoinPlayer != null && HomePlayer != null;
        }
    }
}