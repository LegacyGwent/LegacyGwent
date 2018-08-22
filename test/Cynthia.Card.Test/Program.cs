using System;
using System.Collections.Generic;
using Alsein.Utilities;
using System.Linq;
using Cynthia.Card.Server;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Alsein.Utilities.IO;
using System.Runtime.InteropServices;

namespace Cynthia.Card.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(0);
            var a = new int[] { 0, 1, 2, 3 };
            a = a.Mess().ToArray();
            a.ForAll(Console.WriteLine);
            Console.WriteLine();
            Console.ReadKey();
        }
        private static IDictionary<Group, string> FlavorMap { get; } = new Dictionary<Group, string>
        {
            { Group.Leader, "领袖" },
            { Group.Gold,"金"},
            {Group.Silver,"银"},
            {Group.Copper,"铜"}
        };
        private static void PrintCard(GwentCard card) => Console.WriteLine($"你的卡牌战力为{card.Strength},是一张~{FlavorMap[card.Group]}卡~");
    }
}
