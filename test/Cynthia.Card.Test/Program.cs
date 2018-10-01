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
            await Task.CompletedTask;
            var list = new List<int>() { 1, 8, 4, 5, 6, 7, 9, 4, 3, 1, 5, 7, 6, 4, 5 };
            var list2 = new List<int>() { 1, 2, 3, 4, 5, 4, 3, 2, 1 };
            //var all = new List<IList<int>>() { list, list2 };
            list2.TakeWhile(x => x >= 3).ForAll(x => System.Console.Write("take" + x + "."));
            list2.SkipWhile(x => x >= 3).ForAll(x => System.Console.Write("skip" + x + "."));

            Console.ReadLine();
        }
    }
}
