using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Cynthia.Card.Server
{
    public class GwentDatabaseService
    {
        private readonly IServiceProvider _provider;
        // public IDatabaseService Database { get; set; }
        // private IRepository<UserInfo> _collection;
        private IMongoClient GetMongoClient()
        {
            var result = (IMongoClient)_provider.GetService(typeof(IMongoClient));
            return result;
        }
        private IMongoDatabase GetDatabase() => GetMongoClient().GetDatabase(_dataBaseName);
        private IMongoCollection<UserInfo> GetCollection() => GetDatabase().GetCollection<UserInfo>(_repositoryName);

        private const string _dataBaseName = "gwentdiy";
        private const string _repositoryName = "user";
        public GwentDatabaseService(IServiceProvider provider)
        {
            _provider = provider;
            // Database = database;
            // _collection = Database[_dataBaseName].GetRepository<UserInfo>(_repositoryName);
        }
        public bool AddDeck(string username, DeckModel deck)
        {
            var temp = GetCollection();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            //var user = _collection.AsQueryable().Single(x => x.UserName == username);
            if (user.Decks.Any(x => x.Id == deck.Id))
                return false;
            user.Decks.Add(deck);
            temp.ReplaceOne(x => x.UserName == username, user);
            //_collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool ModifyDeck(string username, string id, DeckModel deck)
        {
            var temp = GetCollection();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            user.Decks[user.Decks.Select((x, index) => (x, index)).Single(d => d.x.Id == id).index] = deck;
            temp.ReplaceOne(x => x.UserName == username, user);
            return true;
        }
        public bool RemoveDeck(string username, string id)
        {
            var temp = GetCollection();
            var user = temp.AsQueryable().Single(x => x.UserName == username);
            user.Decks.RemoveAt(user.Decks.Select((x, index) => (x, index)).Single(deck => deck.x.Id == id).index);
            temp.ReplaceOne(x => x.UserName == username, user);
            // _collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool Register(string username, string password, string playername)
        {
            var temp = GetCollection();
            if (temp.AsQueryable<UserInfo>().Any(x => x.UserName == username || x.PlayerName == playername))
            {
                return false;
            }
            var decks = new List<DeckModel>();
            decks.Add(GwentDeck.CreateBasicDeck(1));
            temp.InsertOne(new UserInfo { UserName = username, PassWord = password, PlayerName = playername, Decks = decks });
            return true;
        }
        public UserInfo Login(string username, string password)
        {
            var temp = GetCollection();
            var user = temp.AsQueryable<UserInfo>().Where(x => x.UserName == username && x.PassWord == password).ToArray();
            return user.Length > 0 ? user[0] : null;
        }

        public IList<GameResult> GetAllGameResults(int count)
        {
            var temp = GetDatabase().GetCollection<GameResult>("gameresults");
            return temp.AsQueryable<GameResult>().OrderByDescending(x => x.Time).Take(count).ToList();
        }
        public bool AddGameResult(GameResult data)
        {
            var temp = GetDatabase().GetCollection<GameResult>("gameresults");
            if (temp.AsQueryable().Any(x => data.Id == x.Id))
            {
                return false;
            }
            temp.InsertOne(data);
            return true;
        }
    }
}