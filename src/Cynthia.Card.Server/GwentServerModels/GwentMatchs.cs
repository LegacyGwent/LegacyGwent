using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentMatchs
    {
        public IList<GwentRoom> GwentRooms { get; set; } = new List<GwentRoom>();
        public void PlayerJoin(GwentClientPlayer player)
        {
            for (var i = 0; i < GwentRooms.Count; i++)
            {
                if (!GwentRooms[i].IsReady)
                {
                    GwentRooms[i].AddPlayer(player);
                    if (GwentRooms[i].IsReady)
                    {
                        var gwentGame = new GwentServerGame(GwentRooms[i].Player1, GwentRooms[i].Player2);
                        GwentRooms[i].Player1.CurrentUser.UserState = UserState.Play;
                        GwentRooms[i].Player2.CurrentUser.UserState = UserState.Play;
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
                if (!GwentRooms[i].IsReady && GwentRooms[i].Player1.CurrentUser.ConnectionId == ConnectionId || GwentRooms[i].Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    GwentRooms.RemoveAt(i);
                    return true;
                }
                if (GwentRooms[i].Player1.CurrentUser.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为JoinPlayer
                    GwentRooms[i].Player2.CurrentUser.UserState = UserState.Standby;
                    GwentRooms[i].Player2.CurrentUser.CurrentPlayer = null;
                    GwentRooms.RemoveAt(i);
                    return true;
                }
                if (GwentRooms[i].Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为HomePlayer
                    GwentRooms[i].Player1.CurrentUser.UserState = UserState.Standby;
                    GwentRooms[i].Player1.CurrentUser.CurrentPlayer = null;
                    GwentRooms.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}