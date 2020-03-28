using System;
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

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cardData = GwentMap.CardMap;
            Console.WriteLine(cardData.GetHashCode());
            var tempData = cardData;
            Console.WriteLine(tempData.GetHashCode());
            Console.ReadLine();
            await Task.CompletedTask;
        }

        static void SaveCardMap()
        {
            var cardData = GwentMap.CardMap;
            var jsonString = JsonConvert.SerializeObject(cardData);
            Console.WriteLine(jsonString);
            Console.WriteLine("写入中...");
            var file = new StreamWriter("D:\\a.txt", false);
            file.WriteLine(jsonString);
            file.Close();
            Console.WriteLine("写入完成");
            Console.ReadLine();
        }
    }
}
