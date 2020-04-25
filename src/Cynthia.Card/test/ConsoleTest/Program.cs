using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card;
using Cynthia.Card.Server;
using Newtonsoft.Json;
using System.IO;
using System.Text.Encodings;
using System.Security.Cryptography;
using System;
using System.Dynamic;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<int>() { 0, 1, 2 };
            list.Insert(0, 10);
            foreach (var item in list)
            {
                Console.WriteLine($"{item}");
            }
            Console.ReadLine();
        }
    }
}
