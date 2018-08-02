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
            //////////////////////////////
            await Task.Delay(0);
            var (upstream, downstream) = AsyncDataEndPoint.CreateDuplex();
            await upstream.SendAsync<string>("aaa");
            downstream.Receive += obj => { Console.WriteLine(obj.Result); return Task.Delay(0); };
            (await downstream.ReceiveAsync<string>()).To(Console.WriteLine);
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
