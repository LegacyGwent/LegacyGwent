using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;

namespace Cynthia.Card
{
    public static class GwentDeck
    {
        public static DeckModel CreateBasicDeck()
        {
            var leader = "tl";
            var deck = "tg1".Plural(1)
            .Concat("tg2".Plural(1))
            .Concat("tg3".Plural(1))
            .Concat("tg4".Plural(1))
            .Concat("ts1".Plural(1))
            .Concat("ts2".Plural(1))
            .Concat("ts3".Plural(1))
            .Concat("ts4".Plural(1))
            .Concat("ts5".Plural(1))
            .Concat("ts6".Plural(1))
            .Concat("tc1".Plural(3))
            .Concat("tc2".Plural(3))
            .Concat("tc3".Plural(3))
            .Concat("tc4".Plural(3))
            .Concat("tc5".Plural(3)).ToArray();
            return new DeckModel() { Leader = leader, Deck = deck, Name = "初始卡组" };
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
            if (decks.Where(x => x.Group == Group.Gold).Distinct().Count() == decks.Where(x => x.Group == Group.Gold).Count() ||
                decks.Where(x => x.Group == Group.Silver).Distinct().Count() == decks.Where(x => x.Group == Group.Silver).Count())
                return false;
            //判断条件5.铜卡重名卡小于3
            if (decks.Where(x => x.Group == Group.Copper).GroupBy(x => x.Id).Select(x => x.Count()).Any(x => x > 3))
                return false;

            return true;
        }
    }
}