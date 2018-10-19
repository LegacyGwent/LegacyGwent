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
            IList<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var s = list.Where(x => x == 5);
            if (s.Count() != 0)
            {
                LogicCardMove
                (
                    list, list.IndexOf(s.First()),
                    list, 0
                );
            }
            list.ForAll(x => Console.Write(" ~ " + x));
            Console.ReadLine();
        }
        public static void LogicCardMove(IList<int> soure, int soureIndex, IList<int> taget, int tagetIndex)
        {
            var item = soure[soureIndex];
            soure.RemoveAt(soureIndex);
            taget.Insert(tagetIndex, item);
        }
    }
}
