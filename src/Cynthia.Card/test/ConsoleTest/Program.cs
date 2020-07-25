using System.Threading.Tasks;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
using System;
using Alsein.Extensions;
using MongoDB.Driver;
using Cynthia.Card;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using Alsein.Extensions.Extensions;
using System.IO.Compression;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var list = new IList<int>[]
            {
                new List<int>{ 1, 2, 3 },
                new List<int>{ 2 },
                new List<int>{ 5 , 3},
            };

            var row = list.Indexed().OrderBy(x => x.Value.Count).First().Key.IndexToMyRow();

            Console.WriteLine(row.ToString());

            await GwentTest.ConfirmExit();
        }
    }
}
