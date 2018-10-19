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
            //await Task.CompletedTask;
            var task1 = Test(5);
            var task2 = Test(5);
            var result = await Task.WhenAll(task1, task2);
            var result1 = task1.Result;
            var result2 = task2.Result;

            Console.ReadLine();
        }
        public static async Task<(int, string)> Test2(int num)
        {
            var task1 = Test(5);
            var task2 = Test("233");
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }
        public static async Task<int> Test(int num)
        {
            await Task.Delay(1000);
            return num;
        }
        public static async Task<string> Test(string s)
        {
            await Task.Delay(1000);
            return s;
        }
    }
}
