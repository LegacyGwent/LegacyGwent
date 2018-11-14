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
            receiver.Receive+=async (x)=>
            {
                await Task.Delay(((int)x.Result)*500);
                Console.WriteLine($"这是等待{((int)x.Result)}秒后输出的消息");
            };
            await sender.SendAsync<int>(5);
            await sender.SendAsync<int>(4);
            await sender.SendAsync<int>(3);
            await sender.SendAsync<int>(2);
            await sender.SendAsync<int>(3);
            //Console.WriteLine($"接收到值:{await receiver.ReceiveAsync<int>()}");
            //Console.WriteLine($"接收到值:{await receiver.ReceiveAsync<int>()}");
            Console.ReadLine();
        }
    }
}