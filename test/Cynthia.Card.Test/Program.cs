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
            var list = new List<int>() { 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1 };
            var ji = 4;
            var ti = ji;
            for (var i = 0; i <= ti; i++)
            {
                if (list[i] == 0)
                    ti++;
            }
            Console.WriteLine(ti);
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
