using System;
using Cynthia.Card.Client;
using Cynthia.Card.Common;
using Cynthia.Card.Server;
using Cynthia.Algorithms;
using System.Collections.Generic;
using Alsein.Utilities;
using System.Linq;

namespace Cynthia.Card.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var deck = new GwentDeck();
            deck.Leader.Plural().Concat(deck.Mess()).ForAll(PrintCard);
        }
        private static IDictionary<Flavor, string> FlavorMap { get; } = new Dictionary<Flavor, string>
        {
            { Flavor.Leader, "领袖" },
            { Flavor.Gold,"金"},
            {Flavor.Silver,"银"},
            {Flavor.Copper,"铜"}
        };
        private static void PrintCard(GwentCard card) => Console.WriteLine($"你的卡牌战力为{card.Strength},是一张~{FlavorMap[card.Flavor]}卡~");
    }
}
