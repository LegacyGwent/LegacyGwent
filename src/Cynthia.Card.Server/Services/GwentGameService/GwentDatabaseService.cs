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
        public bool Login(string username, string password)
        {
            //  _collection.AsQueryable<UserInfo>();
            return true;
        }
    }
}