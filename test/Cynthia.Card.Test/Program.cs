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
        public static void Main(string[] args)
        {
            var list = new List<Test>();
            list.Add(new Test(1,"苹果"));
            list.Add(new Test(1,"香蕉"));
            list.Add(new Test(1,"橘子"));
            list.Add(new Test(2,"汽车"));
            list.Add(new Test(2,"飞机"));
            list.Add(new Test(2,"潜艇"));
            list.GroupBy(x=>x.Key).ForAll(x=>Console.WriteLine(x.Key+"总共有"+x.Count()+"个"));
            Console.ReadLine();
        }
    }
    class Test
    {
        public int Key{get;set;}
        public string Value{get;set;}
        public Test(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}