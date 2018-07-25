using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentMatchs
    {
        public List<GwentRoom> GwentRooms { get; set; } = new List<GwentRoom>();
        public void PlayerJoin(GwentServerPlayer player)
        {
            for (var i = 0; i < GwentRooms.Count; i++)
            {
                if (!GwentRooms[i].IsReady())
                {
                    GwentRooms[i].AddPlayer(player);
                    if (GwentRooms[i].IsReady())
                    {
                        var gwentGame = new GwentServerGame(GwentRooms[i].HomePlayer, GwentRooms[i].JoinPlayer);
                        _ = Task.Run(gwentGame.Play);
                        return;
                    }
                }
            }
            GwentRooms.Add(new GwentRoom(player));
            return;
        }
        public bool PlayerLeave(string ConnectionId)
        {
            for (var i = 0; i < GwentRooms.Count; i++)
            {
                if (!GwentRooms[i].IsReady() && GwentRooms[i].HomePlayer.ConnectionId == ConnectionId || GwentRooms[i].JoinPlayer.ConnectionId == ConnectionId)
                {
                    GwentRooms.RemoveAt(i);
                    return true;
                }
                if (GwentRooms[i].HomePlayer.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为JoinPlayer
                    GwentRooms.RemoveAt(i);
                    return true;
                }
                if (GwentRooms[i].JoinPlayer.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为HomePlayer
                    GwentRooms.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}