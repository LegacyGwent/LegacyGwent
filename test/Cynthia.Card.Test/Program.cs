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
            for (var i = 2; i <= 4; i++)
            {
                if (i == 3) continue;
                System.Console.WriteLine(i + ",");
            }
            System.Console.Read();
        }
        public static Task Test()
        {
            1.To(50).ForAll(Console.Write);
            return Task.CompletedTask;
        }
    }
}
