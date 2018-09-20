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
            IList<GCard> Hand = new List<GCard>()
            {
                new GCard()
                {
                    CStatus = new CStatus()
                    {
                        HP = 5
                    }
                }
            }.ForAll(x => { x.CEffect = new CEffect(x); }).ToList();

            Console.Read();
        }
    }
    public class GCard
    {
        public CEffect CEffect { get; set; }
        public CStatus CStatus { get; set; }
    }
    public class CEffect
    {
        public CEffect(GCard gCard)
        {
            GCard = gCard;
        }
        public GCard GCard { get; set; }
    }
    public class CStatus
    {
        public int HP { get; set; }
    }
}
