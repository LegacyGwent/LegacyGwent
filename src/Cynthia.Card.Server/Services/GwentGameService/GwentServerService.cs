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
        private readonly GwentMatchs _gwentMatchs = new GwentMatchs();
        private readonly IDictionary<string, UserInfo> _users = new Dictionary<string, UserInfo>();
        public bool Login(UserInfo user, string password)
        {
            //判断登录条件
            if (_users.Any(x => x.Value.PlayerName == user.PlayerName))
            {
                return false;
            }
            _users.Add(user.ConnectionId, user);
            return true;
        }
        public bool Register(UserInfo user, string password)
        {
            //判断是否能正确注册
            return true;
        }
        public bool Match(string connectionId, int cardIndex)//匹配
        {
            //判断卡组是否符合规范
            try
            {
                var user = _users.Single(item => item.Key == connectionId && item.Value.UserState == UserState.Standby).Value;
                var player = user.CurrentPlayer = new GwentClientPlayer(user, Container.Resolve<IHubContext<GwentHub>>);
                player.Deck = new GwentDeck();
                _gwentMatchs.PlayerJoin(player);
                _users[connectionId].UserState = UserState.Match;
            }
            catch
            {
                return false;
            }
            return true;
        }
        public Task GameOperation(Operation<UserOperationType> operation, string connectionId) => _users[connectionId].CurrentPlayer.SendToUpstreamAsync(operation);
        public void Disconnect(string connectionId)
        {
            if (_users[connectionId].UserState != UserState.Standby)
            {
                _gwentMatchs.PlayerLeave(connectionId);
            }
            _users.Remove(connectionId);
        }
    }
}