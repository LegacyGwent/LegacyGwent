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
            var test = (Faction[])Enum.GetValues(typeof(Faction));
            test.ForAll(x=>Console.WriteLine(x));
            await Task.CompletedTask;
            Console.ReadLine();
        }
    }
}