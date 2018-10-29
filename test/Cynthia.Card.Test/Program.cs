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
            IList<int> list = new List<int>() { };
            list.Take(5).Count().To(Console.Write);
            Console.ReadLine();
        }
        public static Task WaitUnit(Func<bool> func)
        {
            return Task.Run(() => { while (!func()) ; Console.WriteLine("条件满足"); });
        }
    }
}
