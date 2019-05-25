using System;
using System.Collections.Generic;
using Alsein.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Alsein.Utilities.IO;
using System.Runtime.InteropServices;
using System.Net;

namespace Cynthia.Card.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;
            var rd = new Random();
            for(var i = 0; i<100; i++)
            {
                var a = new List<int>();
                a.Add(0);
                //a.Add(1);
                a.Add(2);
                var rowIndex = a[rd.Next(0,a.Count)];
                Console.WriteLine(rowIndex.IndexToMyRow());
            }
            Console.ReadLine();
        }
    }
}