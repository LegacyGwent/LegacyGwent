using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    public static class CommonFunctionalExtensions
    {
        public static DeckModel ToDeckModel(this GameDeck deck)
        {
            var deckResult = new DeckModel();
            deckResult.Name = deck.Name;
            deckResult.Id = deck.Id;
            deckResult.Leader = deck.Leader.CardId;
            deckResult.Deck = deck.Deck.Select(x => x.CardId).ToList();
            return deckResult;
        }

        public static DeckModel ToDeckModel(this BlacklistModel blacklist)
        {
            var deckResult = new DeckModel();
            deckResult.Name = blacklist.Name;
            deckResult.Id = blacklist.Id;
            deckResult.Leader = "41001"; // use King Radovid V as a fake leader
            deckResult.Deck = blacklist.Blacklist;
            return deckResult;
        }

        public static BlacklistModel ToBlacklist(this DeckModel deck)
        {
            var blacklist = new BlacklistModel();
            blacklist.Name = deck.Name;
            blacklist.Id = deck.Id;
            blacklist.Blacklist = deck.Deck;
            return blacklist;
        }

        public static string CompressDeck(this GameDeck deck) => deck.ToDeckModel().CompressDeck();

        public static string CompressDeck(this DeckModel deck)
        {
            var list = new List<string>();
            var list3 = new List<char> { '(', '[', '{' };
            var newList = list.Concat(deck.Deck).Select(x => GwentMap.CardIdMap[x]).OrderBy(x => x).ToList();
            // newList.Insert(0, map[deck.Leader]);
            var sList = newList.Select(x => ((int)x).To64())
                .GroupBy(x => x)
                .Select(x => new { CardCount = x.Count(), Card62Id = x.Key, Length = x.Key.Length })
                .GroupBy(x => x.CardCount)
                .Select(x => new { Count = x.Key, Cards = x.GroupBy(x => x.Length).OrderBy(x => x.Key) })
                .OrderBy(x => x.Count);

            var result = ((int)GwentMap.CardIdMap[deck.Leader]).To64();

            foreach (var group in sList)
            {
                result += group.Count;
                foreach (var cards in group.Cards)
                {
                    result += list3[cards.Key - 1];
                    result += cards.Select(x => x.Card62Id).Join();
                }
            }
            return result;//.AsSpan().Encode();
        }

        public static string Card64ToCardId(string cardCode)
        {
            // try
            // {
            var index = cardCode.To10();
            return GwentMap.CardIdIndexMap[index];
            // }
            // catch
            // {
            //     Console.WriteLine($"{cardCode}");
            //     return "35002";
            // }
        }
        public static DeckModel DeCompressToDeck(this string stringDeck)
        {
            var deckResult = new DeckModel() { Name = "卡组码卡组(编辑卡组界面点击卡组名可改名哟)" };
            //如果没有任何一个数字,说明只有领袖
            if (!stringDeck.Any(x => (x >= '0' && x <= '9')))
            {
                deckResult.Leader = stringDeck.Length == 0 ? "41001" : Card64ToCardId(stringDeck);
                return deckResult;
            }
            else
            {
                var index = stringDeck.IndexOfAny("0123456789".ToArray());
                deckResult.Leader = Card64ToCardId(stringDeck.Substring(0, index));
                stringDeck = stringDeck.Substring(index, stringDeck.Length - index);
            }

            var group = new List<(int, string)>();
            var tempNumber = "";
            var tempString = "";
            foreach (var item in stringDeck)
            {
                if (item >= '0' && item <= '9')
                {
                    if (tempString != "")
                    {
                        group.Add((int.Parse(tempNumber), tempString));
                        tempNumber = "";
                        tempString = "";
                    }
                    tempNumber += item;
                }
                else
                {
                    tempString += item;
                }
            }
            group.Add((int.Parse(tempNumber), tempString));

            foreach (var item in group)
            {
                var tempStr = item.Item2.Replace("(", " (");
                tempStr = tempStr.Replace("[", " [");
                tempStr = tempStr.Replace("{", " {");
                var result = tempStr.Trim().Split(' ');
                foreach (var s in result)
                {
                    var count = s[0] == '(' ? 1 : (s[0] == '[' ? 2 : 3);
                    for (var i = 1; i < s.Length; i += count)
                    {
                        var right = i + count > s.Length ? s.Length - i : count;
                        deckResult.Deck.AddRange(Card64ToCardId(s.Substring(i, right)).Plural(item.Item1));
                    }
                }
            }

            return deckResult;
        }
    }
}
