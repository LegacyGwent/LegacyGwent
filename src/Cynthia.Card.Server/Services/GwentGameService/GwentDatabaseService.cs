using System;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card.Server
{
    public class GwentDatabaseService
    {
        public IDatabaseService Database { get; set; }
        private IRepository<UserInfo> _collection;
        private const string _dataBaseName = "gwent";
        private const string _repositoryName = "user";
        public GwentDatabaseService(IDatabaseService database)
        {
            Database = database;
            _collection = Database[_dataBaseName].GetRepository<UserInfo>(_repositoryName);
        }
        public bool AddDeck(string username, DeckModel deck)
        {
            var user = _collection.AsQueryable().Single(x => x.UserName == username);
            user.Decks.Add(deck);
            _collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool ModifyDeck(string username, int deckIndex, DeckModel deck)
        {
            var user = _collection.AsQueryable().Single(x => x.UserName == username);
            user.Decks[deckIndex] = deck;
            _collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool RemoveDeck(string username, int deckIndex)
        {
            var user = _collection.AsQueryable().Single(x => x.UserName == username);
            user.Decks.RemoveAt(deckIndex);
            _collection.Update(x => x.UserName == username, user);
            return true;
        }
        public bool Register(string username, string password, string playername)
        {
            if (_collection.AsQueryable<UserInfo>().Any(x => x.UserName == username || x.PlayerName == playername))
            {
                return false;
            }
            var decks = new List<DeckModel>();
            decks.Add(GwentDeck.CreateBasicDeck());
            decks.Add(GwentDeck.CreateBasicDeck());
            decks.Add(GwentDeck.CreateBasicDeck());
            _collection.Add(new UserInfo { UserName = username, PassWord = password, PlayerName = playername, Decks = decks });
            return true;
        }
        public UserInfo Login(string username, string password)
        {
            var user = _collection.AsQueryable<UserInfo>().Where(x => x.UserName == username && x.PassWord == password).ToArray();
            return user.Length > 0 ? user[0] : null;
        }
    }
}