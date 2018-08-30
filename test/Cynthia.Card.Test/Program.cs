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
            await Task.Delay(0);
            for (var i = -3; i <= 5; i++)
            {
                var enemyRowIndex = i == -3 ? i :
                (
                    i == -2 ? -1 :
                    (
                        i == -1 ? -2 :
                        (
                            i >= 3 ? i - 3 : i + 3
                        )
                    )
                );
                Console.WriteLine($"原始数据:{i},加工数据:{enemyRowIndex}");
            }
            Console.ReadKey();
        }

    }
}
