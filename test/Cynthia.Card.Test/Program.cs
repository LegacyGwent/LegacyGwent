using System;
using System.Collections.Generic;
using Alsein.Utilities;
using System.Linq;
using Cynthia.Card.Server;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Alsein.Utilities.IO;

namespace Cynthia.Card.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(0);
            IList<int> list = new List<int>();
            list.Add(1);
            list.Add(3);
            list.Add(5);
            list.Add(7);
            list.Add(2);
            list.Add(4);
            list.Add(6);
            list.Add(8);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{list.Count()},{list.Count}", ConsoleColor.Green);
            Console.ReadKey();
        }
        public static IEnumerable<object> Test(params object[] model)
        {
            return model;
        }
        public static UserInfo Login(string username, string password)
        {
            var users = new List<UserInfo>();
            users.Add(new UserInfo() { UserName = "gezi", PassWord = "233", PlayerName = "baka" });
            users.Add(new UserInfo() { UserName = "hfzi", PassWord = "466", PlayerName = "huaka" });
            users.Add(new UserInfo() { UserName = "itzi", PassWord = "699", PlayerName = "ayay" });
            var user = users.Where(x => x.UserName == username && x.PassWord == password).ToArray();
            return user.Length == 0 ? null : user[0];
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
