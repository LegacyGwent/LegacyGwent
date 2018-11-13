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
            (IAsyncDataSender sender,IAsyncDataReceiver receiver) = AsyncDataEndPoint.CreateSimplex();
            receiver.Receive+=x=>
            {
                if(((int)x.Result)>5)
                {
                    Console.WriteLine($"内部触发,并且不会捉住:{((int)x.Result)}");
                    x.IsMonopolied = true;
                }
                else
                {
                    x.IsMonopolied = false;
                }
                return Task.CompletedTask;
            };
            await sender.SendAsync<int>(3);
            await sender.SendAsync<int>(8);
            Console.WriteLine($"接收到值:{await receiver.ReceiveAsync<int>()}");
            Console.WriteLine($"接收到值:{await receiver.ReceiveAsync<int>()}");
            Console.ReadLine();
        }
    }
}