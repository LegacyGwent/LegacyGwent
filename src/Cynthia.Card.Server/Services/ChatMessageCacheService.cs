using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;
using MongoDB.Driver;
using Cynthia.Card.Common.Models;
using System.Collections.Generic;
using Cynthia.Card.Common;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Server.Services
{
    [Singleton]
    public class ChatMessageCacheService
    {
        public IDatabaseService Client { get; set; }
        public IMessagesService Massage { get; set; }
        private const string dataBaseName = "chat";
        private const string repositoryName = "test";
        private IRepository<ChatMessage> repository;
        private int _strategy = 0;//缓存中的第几位开始,是数据库没有的数据
        public async void AutoSaveData(int minute = 10)
        {
            while (true)
            {
                await Task.Delay(60000 * minute);//为了测试设置为10分钟
                SaveData();
            }
        }
        public void InitData(int initnum = 60)
        {
            repository = GetRepositroy<ChatMessage>();
            //服务开始时,先从数据库取出末尾60条数据
            Massage.AddMessage(GetEndData(initnum));
        }
        public void SaveData()
        {
            //将缓存中多余的数据存入数据库,用_strategy做标识并更新_strategy的值
            Massage.GetMessage(0).Skip(_strategy).To(MessagesToDatabase);
            _strategy = Massage.Count;
        }
        public void MessagesToDatabase(IEnumerable<ChatMessage> messages)
        {
            //将集合中所有元素添加进数据库
            repository.Add(messages);
        }
        public void MessageToDatabase(ChatMessage message)
        {
            //将集合中所有元素添加进数据库
            repository.Add(message);
        }
        public IRepository<TModel> GetRepositroy<TModel>() where TModel : IModel
        {
            //获得数据库集合
            return Client[dataBaseName].GetRepository<TModel>(repositoryName);
        }
        public IEnumerable<ChatMessage> GetEndData(int count)
        {
            //获得数据库末尾的count条数据
            var data = repository.AsQueryable<ChatMessage>().OrderByDescending(x => x.Time).Take(count).OrderBy(x => x.Time).AsEnumerable();
            _strategy = data.Count();
            return data;
        }
        public int GetDataCount()
        {
            //获得数据库中总共的数据数量
            return repository.Count() + Massage.Count - _strategy;
        }
        public IEnumerable<ChatMessage> GetPageData(int page, int count = 60)
        {
            //获得某一页的数据  (每个的数量,和第几页  默认一页60条数据
            var pagecount = GetPageNum(count);
            return repository.AsQueryable<ChatMessage>().OrderBy(x => x.Time).Skip(page * count).Take(count).AsEnumerable();
        }
        public int GetPageNum(int count = 60)
        {
            //获得总共的页数  默认一页60条数据
            var datacount = GetDataCount();
            if (datacount % count != 0)
            {
                return datacount / count + 1;
            }
            return datacount / count;
        }
    }
}