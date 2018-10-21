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
        public static async Task Main(string[] args)
        {
            await Task.CompletedTask;
            var list = new List<int>() { 9, 1, 8, 2, 7, 3, 4, 6, 5, 10, 0, 0, 0, 89 };
            LinqTest(list, x => x > 6).ForAll(Console.WriteLine);
            Console.ReadLine();
        }
        public static IList<int> LinqTest(IList<int> list, Func<int, bool> sizer)
        {
            var result = new List<int>();
            list.Select((x, index) => (item: x, index: index)).Where(x => sizer(x.item)).ForAll(x => result.Add(x.index));
            return result;
        }
    }
}
