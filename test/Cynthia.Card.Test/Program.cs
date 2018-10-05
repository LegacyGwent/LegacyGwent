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
            //1.To(9).Select(x => 1.To(x).Select(y => $"{x}x{y}={x * y}").Join("\t")).ForAll(Console.WriteLine);
            /*var size = 10;
            size.To(1)
            .Select(x => 1.To(x).Select(a => " ")
            .Concat(1.To((size - x + 1) * 2 - 1).Select(b => "x")).Join(""))
            .Plural(2).SelectMany(x => x)
            .ForAll(Console.WriteLine);
            */
            1.To(10).ForAll(x => Console.WriteLine(new Random().Next(2)));
            Console.ReadLine();
        }
    }
}
