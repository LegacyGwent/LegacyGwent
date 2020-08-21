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
            GwentTest.QueryPlayerDeck("岁月");

            await GwentTest.ConfirmExit();
        }
    }
}
