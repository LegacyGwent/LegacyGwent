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
            var source = new TaskCompletionSource<int>();
            var task = source.Task;
            source.SetResult(5);
            var result = await task;
            Console.WriteLine(result);
            Console.ReadLine();
        }
        public static Task WaitUnit(Func<bool> func)
        {
            return Task.Run(() => { while (!func()) ; Console.WriteLine("条件满足"); });
        }
        public static void LogicCardMove(IList<int> soure, int soureIndex, IList<int> taget, int tagetIndex)
        {
            var item = soure[soureIndex];
            soure.RemoveAt(soureIndex);
            taget.Insert(tagetIndex > taget.Count ? taget.Count : tagetIndex, item);
        }
    }
}
