using System;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    //一个卡牌类型
    public struct GwentCard
    {
        [Key]
        public string CardId { get; set; }
        // public Type EffectType { get; set; }

        //包含标签  例如:士兵,精灵 (多选)
        public Categorie[] Categories { get; set; }


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

        public int Countdown { get; set; }

        public bool IsCountdown { get; set; }

        public bool IsDerive { get; set; }
    }
}