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
            IList<int> l1 = new List<int> { 1, 2, 3, 4, 5, 6 };
            IList<int> l2 = new List<int> { 7, 8, 9, 10, 11, 12 };
            CardMove<int>(l1, l1.Count - 1, l2, l2.Count - 1);
            l1.ForAll(x => Console.Write($"{x}+"));
            Console.WriteLine();
            l2.ForAll(x => Console.Write($"{x}+"));
            Console.Read();
        }

        static public T CardMove<T>(IList<T> soure, int soureIndex, IList<T> taget, int tagetIndex)
        {
            var item = soure[soureIndex];
            soure.RemoveAt(soureIndex);
            taget.Add(item);
            return item;
        }
    }
}
