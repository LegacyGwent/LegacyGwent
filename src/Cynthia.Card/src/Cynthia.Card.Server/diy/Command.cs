using AntDesign;
using MongoDB.Driver;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Command
{

    static MongoClient client;
    static IMongoDatabase db;
    public static IMongoCollection<DiyCardInfo> diyCardCollection;
    public static IMongoCollection<DiyCardTextureInfo> diyCardTextureCollection;
    public static IMongoCollection<DefaultTexture> defaultTextureCollection;
    public static void MongodbConnect()
    {
        client = new MongoClient("mongodb://cynthia.ovyno.com:28020");
        db = client.GetDatabase("Web");
        diyCardCollection = db.GetCollection<DiyCardInfo>("DiyCards");
        diyCardTextureCollection = db.GetCollection<DiyCardTextureInfo>("DiyCardTexture");
        defaultTextureCollection = db.GetCollection<DefaultTexture>("DefaultTexture");
    }
    public static void InItDefaultTexture(DefaultTexture defaultTexture)
    {
        //defaultTextureCollection.InsertOne(defaultTexture);
    }
    public static void InItDefaultTexture()
    {
        //Info.defaultTexture = defaultTextureCollection.AsQueryable().First();
    }
    public static void InItDiyCardInfos()
    {


        for (int i = 0; i < 10; i++)
        {
            DiyCardInfo diyCard = new DiyCardInfo()
            {
                cardName = "物体" + i,
                describe = "这是" + i + "的描述",
                commits = new List<DiyCardInfo.Commit>
                    {
                        new DiyCardInfo.Commit()
                        {
                            user="gezi",
                            text="好烂"
                        },
                        new DiyCardInfo.Commit()
                        {
                            user="gezi",
                            text="好烂"
                        }
                    }
            };
            diyCardCollection.InsertOne(diyCard);
        }
    }
    public static void AddDiyCardInfos(string name, string describe, byte[] uploadImageData)
    {
        int uid = diyCardCollection.AsQueryable().Count();
        DiyCardInfo diyCard = new DiyCardInfo()
        {
            uid= uid,
            cardName = name,
            describe = describe,
            commits = new List<DiyCardInfo.Commit>
                    {
                        new DiyCardInfo.Commit()
                        {
                            user="gezi",
                            text="好烂"
                        },
                        new DiyCardInfo.Commit()
                        {
                            user="gezi",
                            text="好烂"
                        }
                    }
        };
        DiyCardTextureInfo diyCardTextureInfo = new DiyCardTextureInfo()
        {
            uid = uid,
            imagedata = uploadImageData,
        };
        diyCardCollection.InsertOne(diyCard);
        diyCardTextureCollection.InsertOne(diyCardTextureInfo);
    }
    public static void GetDiyCardsInfo()
    {
        Info.diyCardInfo = diyCardCollection.AsQueryable().ToList();
    }
    public static byte[] GetDiyTexture(int uid)
    {
       return diyCardTextureCollection.Find(info => info.uid == uid).First().imagedata;
    }
}
