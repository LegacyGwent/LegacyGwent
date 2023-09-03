using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Cynthia.Card.Server;

public class Command
{
    public static IMongoClient client;
    static IMongoDatabase db;
    public static IMongoCollection<DiyCardInfo> diyCardCollection;
    public static IMongoCollection<DiyCardInfo> discussAreaCollection;
    public static GwentDatabaseService _dbServer;
    public static void MongodbConnect(GwentDatabaseService dbServer)
    {
        _dbServer = dbServer;
        client = dbServer.GetMongoClient();
        db = client.GetDatabase("Web");
        diyCardCollection = db.GetCollection<DiyCardInfo>("DiyCards");
        discussAreaCollection = db.GetCollection<DiyCardInfo>("DiscussArea");
    }
    public static void AddDiyCardInfos(DiyCardInfo diyCard)
    {
        int uid = diyCardCollection.AsQueryable().Count();
        diyCard.uid = uid;
        diyCard.lastEditedDate = DateTime.Now;
        diyCard.commits = new List<DiyCardInfo.Commit> { };
        diyCard.likeList = new List<string> { };
        diyCard.dislikeList = new List<string> { };
        diyCardCollection.InsertOne(diyCard);
    }
    public static void AddDiscussCardInfos(DiyCardInfo diyCard)
    {
        var filter = Builders<DiyCardInfo>.Filter.Eq(x => x._id, diyCard._id);
        var exists = discussAreaCollection.Find(filter).Any();
        if (!exists)
        {
            int uid = discussAreaCollection.AsQueryable().Count();
            var update = Builders<DiyCardInfo>.Update
                .Set(x => x.likeList, new List<string>())
                .Set(x => x.dislikeList, new List<string>())
                .Set(x => x.commits, new List<DiyCardInfo.Commit>())
                .Set(x => x.uid, uid)
                .Set(x => x.lastEditedDate, DateTime.Now);
            var updateIsInDiscuss = Builders<DiyCardInfo>.Update.Set(x => x.IsInDiscuss, true);
            discussAreaCollection.InsertOne(diyCard);
            discussAreaCollection.UpdateOne(filter, update);
            diyCardCollection.UpdateOne(filter,updateIsInDiscuss);
        }
    }
    public static void RemoveDiyCard(DiyCardInfo diyCard, IMongoCollection<DiyCardInfo> collection)
    {
        var filter = Builders<DiyCardInfo>.Filter.Eq(x => x._id, diyCard._id);
        collection.DeleteOne(filter);
    }
    public static void GetDiyCardsInfo()
    {
        Info.diyCardInfo = diyCardCollection.AsQueryable().ToList();
    }
    public static void GetDiscussCardsInfo()
    {
        Info.diyCardInfo = discussAreaCollection.AsQueryable().ToList();
    }
    public static void UpdateDiyCardComment(DiyCardInfo diyCard, IMongoCollection<DiyCardInfo> collection)
    {
        var filter = Builders<DiyCardInfo>.Filter.Eq("_id", diyCard._id);
        var update = Builders<DiyCardInfo>.Update.Set("commits", diyCard.commits);
        collection.UpdateOne(filter, update);
    }
    public static void UpdateDiyCardLikeList(DiyCardInfo diyCard, IMongoCollection<DiyCardInfo> collection)
    {
        var filter = Builders<DiyCardInfo>.Filter.Eq("_id", diyCard._id);
        var update = Builders<DiyCardInfo>.Update.Set("likeList", diyCard.likeList);
        collection.UpdateOne(filter, update);
    }
    public static void UpdateDiyCardDislikeList(DiyCardInfo diyCard, IMongoCollection<DiyCardInfo> collection)
    {
        var filter = Builders<DiyCardInfo>.Filter.Eq("_id", diyCard._id);
        var update = Builders<DiyCardInfo>.Update.Set("dislikeList", diyCard.dislikeList);
        collection.UpdateOne(filter, update);
    }
    public static bool Login(string username, string password)
    {
        var user = _dbServer.Login(username, password);
        return user != null;
    }
}