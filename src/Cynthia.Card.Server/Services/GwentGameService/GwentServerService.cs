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
            var loginUser = DatabaseService.Login(user.UserName, password);
            if (loginUser != null)
            {
                if (_users.Any(x => x.Value.UserName == user.UserName))//如果重复登录的话,触发"掉线"
                {
                    var connectionId = _users.Single(x => x.Value.UserName == user.UserName).Value.ConnectionId;
                    Container.Resolve<IHubContext<GwentHub>>().Clients.Client(connectionId).SendAsync("RepeatLogin");
                    Disconnect(connectionId);
                }
                if (_users.ContainsKey(user.ConnectionId))
                {
                    Disconnect(user.ConnectionId);
                }
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
        public bool StopMatch(string connectionId)
        {
            return _gwentMatchs.PlayerLeave(connectionId);
        }
        public bool AddDeck(string connectionId, DeckModel deck)
        {
            //添加卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 40)
                return false;
            if (!deck.IsBasicDeck())
                return false;
            if (!DatabaseService.AddDeck(user.UserName, deck))
                return false;
            user.Decks.Add(deck);
            return true;
        }
        public bool RemoveDeck(string connectionId, int deckIndex)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count <= deckIndex || deckIndex < 0)
                return false;
            if (!DatabaseService.RemoveDeck(user.UserName, deckIndex))
                return false;
            user.Decks.RemoveAt(deckIndex);
            return true;
        }
        public bool ModifyDeck(string connectionId, int deckIndex, DeckModel deck)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count <= deckIndex || deckIndex < 0)
                return false;
            if (!deck.IsBasicDeck())
                return false;
            if (!DatabaseService.ModifyDeck(user.UserName, deckIndex, deck))
                return false;
            user.Decks[deckIndex] = deck;
            return true;
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