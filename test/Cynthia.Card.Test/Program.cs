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
            Console.WriteLine("小剧场");
            Console.WriteLine("-------Start-------");
            Console.WriteLine("路人A:唔...稍微有个问题不会呢,但是有很多工作呀,去问问琪露诺好了");
            Task work = BakaAdd(1, 2);//把问题交给琪露诺去完成
            Console.WriteLine("(路人A工作中)");
            Console.WriteLine("(路人A工作中)");//琪露诺思考的过程,可以做一些其他事情
            Console.WriteLine("(路人A工作中)");
            Console.WriteLine("路人A:嗯...这里的工作需要刚刚哪个问题的结果呢,去等琪露诺做好吧!");
            await work;//等待琪露诺的思考
            Console.WriteLine("--------END--------");
            Console.ReadKey();
        }
        public async static Task BakaAdd(int a, int b)
        {
            Console.WriteLine($"琪露诺:{a}+{b}等于多少呢....我要好好想想...");
            await Task.Delay(1000);//琪露诺思考中....
            Console.WriteLine($"琪露诺:啊!{a}+{b}等于⑨呀");
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
