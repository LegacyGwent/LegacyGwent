using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    public static class GwentDeck
    {
        public static DeckModel CreateBasicDeck(int defaultDeckIndex)
        {
            var deck = default(List<string>);
            switch (defaultDeckIndex)
            {
                case 1:
                    deck = "12001".Plural(1)//乞丐王
                    .Concat("12002".Plural(1))//诗人
                    .Concat("12003".Plural(1))//皇家
                    .Concat("13001".Plural(1))//杰洛特
                    .Concat("13002".Plural(1))//林法恩
                    .Concat("33004".Plural(1))//帝国间谍
                    .Concat("32001".Plural(1))//大魔像
                    .Concat("33001".Plural(1))//冒牌希里
                    .Concat("33002".Plural(1))//5+10
                    .Concat("33003".Plural(1))//萝卜
                    .Concat("34005".Plural(3))//医生
                    .Concat("34030".Plural(3))//特使
                    .Concat("34002".Plural(3))//侦查
                    .Concat("34003".Plural(3))//近卫军
                    .Concat("34004".Plural(3)).ToList();//12点
                    return new DeckModel()
                    {
                        Leader = "31001",
                        Deck = deck,
                        Name = "帝国测试卡组",
                        Id = Guid.NewGuid().ToString()
                    };
                default:
                    deck = "12001".Plural(1)//乞丐王
                    .Concat("12002".Plural(1))//诗人
                    .Concat("12003".Plural(1))//皇家
                    .Concat("13001".Plural(1))//杰洛特
                    .Concat("13002".Plural(1))//林法恩
                    .Concat("33004".Plural(1))//帝国间谍
                    .Concat("32001".Plural(1))//大魔像
                    .Concat("33001".Plural(1))//冒牌希里
                    .Concat("33002".Plural(1))//5+10
                    .Concat("33003".Plural(1))//萝卜
                    .Concat("34005".Plural(3))//侦察
                    .Concat("34030".Plural(3))//特使
                    .Concat("34002".Plural(3))//侦查
                    .Concat("34003".Plural(3))//近卫军
                    .Concat("34004".Plural(3)).ToList();//12点
                    return new DeckModel() { Leader = "31001", Deck = deck, Name = "帝国测试卡组", Id = Guid.NewGuid().ToString() };
            }
        }
        public static bool IsBasicDeck(this DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);     //将卡组编号集合,转换成对应的卡
            var deckFaction = GwentMap.CardMap[deck.Leader].Faction;    //得到卡组领袖所在的势力

            //判断条件1. 所有卡牌是否都为本势力卡, 或中立卡
            if (decks.Any(x => x.Faction != Faction.Neutral && x.Faction != deckFaction))
                return false;
            //判断条件2.卡组数量大于25张,小于40张
            if (decks.Count() < 25 || decks.Count() > 40)
                return false;
            //判断条件3.金卡小于等于4张,银卡小于等于6张,无领袖
            if (decks.Where(x => x.Group == Group.Gold).Count() > 4 ||
                decks.Where(x => x.Group == Group.Silver).Count() > 6 ||
                decks.Any(x => x.Group == Group.Leader))
                return false;
            //判断条件4.金卡无重复,银卡无重复
            if (decks.Where(x => x.Group == Group.Gold).Distinct().Count() != decks.Where(x => x.Group == Group.Gold).Count() ||
                decks.Where(x => x.Group == Group.Silver).Distinct().Count() != decks.Where(x => x.Group == Group.Silver).Count())
                return false;
            //判断条件5.铜卡重名卡小于3
            if (decks.Where(x => x.Group == Group.Copper).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3))
                return false;
            return true;
        }
        public static bool IsHalfBasicDeck(this DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);     //将卡组编号集合,转换成对应的卡
            var deckFaction = GwentMap.CardMap[deck.Leader].Faction;    //得到卡组领袖所在的势力

            //判断条件1. 所有卡牌是否都为本势力卡, 或中立卡
            if (decks.Any(x => x.Faction != Faction.Neutral && x.Faction != deckFaction))
                return false;
            //判断条件2.卡组数量小于40张
            if (decks.Count() > 40)
                return false;
            //判断条件3.金卡小于等于4张,银卡小于等于6张,无领袖
            if (decks.Where(x => x.Group == Group.Gold).Count() > 4 ||
                decks.Where(x => x.Group == Group.Silver).Count() > 6 ||
                decks.Any(x => x.Group == Group.Leader))
                return false;
            //判断条件4.金卡无重复,银卡无重复
            if (decks.Where(x => x.Group == Group.Gold).Distinct().Count() != decks.Where(x => x.Group == Group.Gold).Count() ||
                decks.Where(x => x.Group == Group.Silver).Distinct().Count() != decks.Where(x => x.Group == Group.Silver).Count())
                return false;
            //判断条件5.铜卡重名卡小于3
            if (decks.Where(x => x.Group == Group.Copper).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3))
                return false;
            return true;
        }
        public static bool IsSpecialDeck(this DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);
            var deckFaction = GwentMap.CardMap[deck.Leader].Faction;


            if (decks.Any(x => x.Faction != Faction.Neutral && x.Faction != deckFaction))
                return false;

            if (decks.Count() < 25 || decks.Count() > 40)
                return false;

            if (decks.Where(x => x.Group == Group.Gold).Count() > 12 ||
                decks.Where(x => x.Group == Group.Silver).Count() > 6 ||
                decks.Any(x => x.Group == Group.Leader))
                return false;

            if (decks.Where(x => x.Group == Group.Gold).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3) ||
               decks.Where(x => x.Group == Group.Silver).Distinct().Count() != decks.Where(x => x.Group == Group.Silver).Count())
                return false;

            if (decks.Where(x => x.Group == Group.Copper).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3))
                return false;
            return true;
        }
        public static bool IsBlacklist(this DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);
            var deckFaction = GwentMap.CardMap[deck.Leader].Faction;

            if (decks.Count() > 2)
                return false;

            return true;
        }
        public static bool IsHalfSpecialDeck(this DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);
            var deckFaction = GwentMap.CardMap[deck.Leader].Faction;


            if (decks.Any(x => x.Faction != Faction.Neutral && x.Faction != deckFaction))
                return false;

            if (decks.Count() > 40)
                return false;

            if (decks.Where(x => x.Group == Group.Gold).Count() > 12 ||
                decks.Where(x => x.Group == Group.Silver).Count() > 6 ||
                decks.Any(x => x.Group == Group.Leader))
                return false;

            if (decks.Where(x => x.Group == Group.Gold).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3) ||
               decks.Where(x => x.Group == Group.Silver).Distinct().Count() != decks.Where(x => x.Group == Group.Silver).Count())
                return false;

            if (decks.Where(x => x.Group == Group.Copper).GroupBy(x => x.CardId).Select(x => x.Count()).Any(x => x > 3))
                return false;
            return true;
        }
    }
}