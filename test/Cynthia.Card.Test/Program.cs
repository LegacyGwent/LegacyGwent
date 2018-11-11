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
            0.To(10).Append(-1).ForAll(x=>System.Console.Write(x+" "));
            Console.ReadLine();
        }
    }
}