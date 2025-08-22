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
                case 1://alchemy
                    deck = "32013".Plural(1)//cahir
                    .Concat("32006".Plural(1))//vilgeforz
                    .Concat("12026".Plural(1))//Triss tele
                    .Concat("12023".Plural(1))//Vesemir Mentor
                    .Concat("33004".Plural(1))//Cantarella
                    .Concat("33005".Plural(1))//Assire
                    .Concat("13007".Plural(1))//Stregobor
                    .Concat("13001".Plural(1))//Roach
                    .Concat("13023".Plural(1))//Black Blood
                    .Concat("13040".Plural(1))//Mandrake
                    .Concat("34022".Plural(3))//Viper Witcher
                    .Concat("34001".Plural(3))//Vicovaro Novice
                    .Concat("14001".Plural(1))//Scout
                    .Concat("14020".Plural(3))//Mahakam Ale
                    .Concat("34033".Plural(3))//Ointment
                    .Concat("70157".Plural(3)).ToList();//Wisteria
                    return new DeckModel()
                    {
                        Leader = "31001",//calveit
                        Deck = deck,
                        Name = "Alchemy",
                        Id = Guid.NewGuid().ToString()
                    };
                case 2://movement
                    deck = "12009".Plural(1)//Ale of the Ancestors
                    .Concat("52005".Plural(1))//Zoltan Chivay
                    .Concat("52013".Plural(1))//Isengrim: Outlaw
                    .Concat("12001".Plural(1))//Royal Decree
                    .Concat("53015".Plural(1))//Éibhear Hattori
                    .Concat("53017".Plural(1))//Barclay Els
                    .Concat("13031".Plural(1))//Marching Orders
                    .Concat("13040".Plural(1))//Mandrake
                    .Concat("53020".Plural(1))//Pit Trap
                    .Concat("70054".Plural(1))//Feign Death
                    .Concat("70109".Plural(3))//Dwarven Chariot
                    .Concat("54005".Plural(2))//Dwarven Mercenary
                    .Concat("54009".Plural(3))//Dol Blathanna Bowman
                    .Concat("54025".Plural(3))//Blue Mountain Elite
                    .Concat("54029".Plural(2))//Dwarven Agitator
                    .Concat("14001".Plural(2)).ToList();//Scout
                    return new DeckModel()
                    {
                        Leader = "51002",//Eithné
                        Deck = deck,
                        Name = "Movement",
                        Id = Guid.NewGuid().ToString()
                    };
                case 3://Consume
                    deck = "22006".Plural(1)//Whispess: Tribute
                    .Concat("12028".Plural(1))//Phoenix
                    .Concat("22013".Plural(1))//Ge'els
                    .Concat("22014".Plural(1))//Brewess: Ritual
                    .Concat("23018".Plural(1))//Maerolorn
                    .Concat("13024".Plural(1))//Alzur's Double–Cross
                    .Concat("13031".Plural(1))//Marching Orders
                    .Concat("13036".Plural(1))//Summoning Circle
                    .Concat("13040".Plural(1))//Mandrake
                    .Concat("23021".Plural(1))//Monster Nest
                    .Concat("24009".Plural(3))//Nekker Warrior
                    .Concat("24012".Plural(3))//Forktail
                    .Concat("24022".Plural(3))//Vran Warrior
                    .Concat("24031".Plural(3))//Nekker
                    .Concat("24037".Plural(3)).ToList();//Slyzard
                    return new DeckModel()
                    {
                        Leader = "21002",//Arachas Queen
                        Deck = deck,
                        Name = "Consume",
                        Id = Guid.NewGuid().ToString()
                    };
                case 4://Great swords
                    deck = "12002".Plural(1)//Geralt: Igni
                    .Concat("12003".Plural(1))//Dandelion: Poet
                    .Concat("62011".Plural(1))//Coral
                    .Concat("62012".Plural(1))//Hym
                    .Concat("63002".Plural(1))//Udalryk
                    .Concat("63003".Plural(1))//Djenge Frett
                    .Concat("63012".Plural(1))//Harald Houndsnout
                    .Concat("63015".Plural(1))//Skjall
                    .Concat("63017".Plural(1))//Sigrdrifa
                    .Concat("63019".Plural(1))//Restore
                    .Concat("64009".Plural(3))//An Craite Greatsword
                    .Concat("64017".Plural(3))//Dimun Light Longship
                    .Concat("64029".Plural(1))//Heymaey Spearmaiden
                    .Concat("64028".Plural(3))//Dimun Corsair
                    .Concat("64031".Plural(2))//Dimun Pirate Captain
                    .Concat("64032".Plural(3)).ToList();//Priestess of Freya
                    return new DeckModel()
                    {
                        Leader = "61002",//Crach an Craite
                        Deck = deck,
                        Name = "Greatswords",
                        Id = Guid.NewGuid().ToString()
                    };
                case 5://Foltest
                    deck = "42003".Plural(1)//Seltkirk of Gulet
                    .Concat("42006".Plural(1))//Keira Metz
                    .Concat("42008".Plural(1))//Sigismund Dijkstra
                    .Concat("42009".Plural(1))//Shani
                    .Concat("13005".Plural(1))//Germain Piquant
                    .Concat("43011".Plural(1))//Margarita of Aretuza
                    .Concat("43016".Plural(1))//Prince Stennis
                    .Concat("13024".Plural(1))//Alzur's Double–Cross
                    .Concat("13031".Plural(1))//Marching Orders
                    .Concat("43020".Plural(1))//Reinforcements
                    .Concat("44009".Plural(3))//Kaedweni Knight
                    .Concat("44013".Plural(3))//Redanian Knight-Elect
                    .Concat("44030".Plural(3))//Witch Hunter
                    .Concat("44031".Plural(2))//Tormented Mage
                    .Concat("44032".Plural(1))//Reaver Scout
                    .Concat("14003".Plural(1))//Alzur's Thunder
                    .Concat("14018".Plural(2)).ToList();//Thunderbolt
                    return new DeckModel()
                    {
                        Leader = "41003",//King Foltest
                        Deck = deck,
                        Name = "Foltes Tempo",
                        Id = Guid.NewGuid().ToString()
                    };
                default://old default alchemy
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