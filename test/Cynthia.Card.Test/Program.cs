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
            await Task.CompletedTask;
            IList<int> list = new List<int>() { };
            list.Take(5).Count().To(Console.Write);
            Console.ReadLine();
        }
        public static Task WaitUnit(Func<bool> func)
        {
            return Task.Run(() => { while (!func()) ; Console.WriteLine("条件满足"); });
        }
    }
    public interface TestInterface
    {
        int Test(int num);
        int Test2(int num);
    }
    public abstract class TestClass2 : TestInterface
    {
        public virtual int Test(int num)
        {
            return num * num;
        }
        public virtual int Test2(int num)
        {
            return num * num;
        }
    }
    public class TestClass1 : TestClass2
    {
        public override int Test(int num)
        {
            return num + num;
        }
    }
}
