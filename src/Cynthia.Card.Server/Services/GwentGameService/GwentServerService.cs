using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;
using Alsein.Utilities.LifetimeAnnotations;
using Cynthia.Card.Common;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    [Singleton]
    public class GwentServerService
    {
        public IContainer Container { get; set; }
        public GwentMatchs GwentMatchs { get; set; }
        public IDictionary<string, UserInfo> Users { get; set; }
        public bool Login(UserInfo user, string password)
        {
            //判断登录条件
            Users.Add(user.ConnectionId, user);
            return true;
        }
        public bool Register(UserInfo user, string password)
        {
            //判断是否能正确注册
            Users.Add(user.ConnectionId, user);
            return true;
        }
        public bool Match(string connectionId, int cardIndex)//匹配
        {
            //判断卡组是否符合规范
            try
            {
                var user = Users.Single(item => item.Key == connectionId && !item.Value.IsPlay).Value;
                var player = user.CurrentPlayer = new GwentServerPlayer(user, Container.Resolve<IHubContext<GwentHub>>);
                player.Deck = new GwentDeck();
                GwentMatchs.PlayerJoin(player);
                Users[connectionId].IsPlay = true;
            }
            catch
            {
                return false;
            }
            return true;
        }
        public Task GameOperation(Operation<ClientOperationType> operation, string connectionId) => Users[connectionId].CurrentPlayer.UserOperation(operation);
        public void Disconnect(string connectionId)
        {
            if (Users[connectionId].IsPlay)
            {
                GwentMatchs.PlayerLeave(connectionId);
            }
            Users.Remove(connectionId);
        }
    }
}