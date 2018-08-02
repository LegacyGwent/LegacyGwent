using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    [Singleton]
    public class GwentServerService
    {
        public IContainer Container { get; set; }
        public GwentDatabaseService DatabaseService { get; set; }
        private readonly GwentMatchs _gwentMatchs = new GwentMatchs();
        private readonly IDictionary<string, User> _users = new Dictionary<string, User>();
        public UserInfo Login(User user, string password)
        {
            //判断登录条件
            if (_users.Any(x => x.Value.UserName == user.UserName)) { return null; }
            var loginUser = DatabaseService.Login(user.UserName, password);
            if (loginUser != null)
            {
                user.PlayerName = loginUser.PlayerName;
                user.Decks = loginUser.Decks;
                _users.Add(user.ConnectionId, user);
            }
            return loginUser;
        }
        public bool Register(string username, string password, string playerName) => DatabaseService.Register(username, password, playerName);
        public bool Match(string connectionId, int deckIndex)//匹配
        {
            if (_users.ContainsKey(connectionId))
            {
                var user = _users[connectionId];
                if (user.UserState != UserState.Standby || user.Decks.Count <= deckIndex || deckIndex < 0)
                    return false;
                var player = user.CurrentPlayer = new ClientPlayer(user, Container.Resolve<IHubContext<GwentHub>>);
                player.Deck = user.Decks[deckIndex];
                _gwentMatchs.PlayerJoin(player);
                return true;
            }
            return false;
        }
        public Task GameOperation(Operation<UserOperationType> operation, string connectionId) => _users[connectionId].CurrentPlayer.SendAsync(operation);
        public void Disconnect(string connectionId)
        {
            if (!_users.ContainsKey(connectionId))
                return;
            if (_users[connectionId].UserState != UserState.Standby)
            {
                _gwentMatchs.PlayerLeave(connectionId);
            }
            _users.Remove(connectionId);
        }
    }
}