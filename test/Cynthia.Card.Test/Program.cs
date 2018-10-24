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
            var a = 1;
            var task = WaitUnit(() => a == 3);
            for (a = 0; a < 5; a++)
            {
                Console.WriteLine($"将a的值改为了{a}");
            }
            await task;
            Console.ReadLine();
        }
        public static Task WaitUnit(Func<bool> func)
        {
            return Task.Run(() => { while (!func()) ; Console.WriteLine("条件满足"); });
        }
    }
}
