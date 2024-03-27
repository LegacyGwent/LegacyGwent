using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Cynthia.Card;

public class DiyCardInfo
{
    public ObjectId _id;
    public int uid;
    public SavedGwentCard cardInfo;
    public string Fraction;
    public string CardType;
    public string Group;
    public List<string> Categories;
    public string imageLink;
    public string author;
    public DateTime lastEditedDate;
    public List<string> likeList;
    public List<string> dislikeList;
    public List<Commit> commits = new List<Commit>();
    public bool IsInDiscuss { get; set; } = false;
    public class Commit
    {
        public string user;
        public string text;
        public List<string> likeList;
        public List<string> dislikeList;
        public DateTime commitDate;
    }
    public class SavedGwentCard
    {
        public string CardId { get; set; }

        //包含标签  例如:士兵,精灵 (多选)
        public Categorie[] Categories { get; set; }

        //一些需要的隐藏标签
        public HideTag[] HideTags { get; set; }


        //所属势力  例如:松鼠党
        public Faction Faction { get; set; }


        //卡牌描述  例如:管他泰莫利亚人还是瑞达尼亚人，杀无赦。
        public string Flavor { get; set; }


        //卡牌品质  例如:铜卡
        public Group Group { get; set; }


        //能力描述  例如:使所有“精灵”友军获得1点增益。 每次被交换时，再次触发此能力。
        public string Info { get; set; }

        //卡牌名称  例如:维里赫德旅先锋
        public string Name { get; set; }

        //战力数值  例如:6
        public int Strength { get; set; }

        //可放置
        public CardUseInfo CardUseInfo { get; set; }

        //卡牌类型
        public CardType CardType { get; set; }

        //卡牌效果的索引
        public string CardArtsId { get; set; }

        //是否佚亡
        public bool IsDoomed { get; set; }

        //初始倒计时
        public int Countdown { get; set; }

        //是否开启倒数显示
        public bool IsCountdown { get; set; }

        //是否是属于衍生卡池
        public bool IsDerive { get; set; }

        public int CrewCount { get; set; }

        public bool IsConcealCard { get; set; }

    }
}
public class AdminInfo
{
    public ObjectId _id;
    public int uid;
    // public int cabinetDate;
    public int cabinetYear = DateTime.Now.Year;
    public int cabinetMonth = DateTime.Now.Month;
    public List<string> cabinetUserName = new List<string>{};
}