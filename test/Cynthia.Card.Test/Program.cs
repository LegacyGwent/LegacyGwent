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
            var a = new int[] { 0, 1, 2, 3 };
            a = a.Mess().ToArray();
            a.ForAll(Console.WriteLine);
            Console.WriteLine();
            Console.ReadKey();
        }

    }
}
