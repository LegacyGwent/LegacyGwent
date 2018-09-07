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
        static List<Task> TaskList = new List<Task>();
        static async Task Main(string[] args)
        {
            await Task.Delay(0);
            _ = TaskTest(1000, "msg1");
            _ = TaskTest(800, "msg2");
            _ = TaskTest(600, "msg3");
            _ = TaskTest(400, "msg4");
            _ = TaskTest(200, "msg5");
            _ = TaskTest(800, "msg6");
            Console.ReadKey();
        }
        static async Task TaskTest(int delay, object message)
        {
            await Task.Delay(delay);
            Console.WriteLine(message);
        }
        static async void TaskListTask(int delay, object message)
        {
            await Task.Delay(0);
            TaskList.Add(TaskTest(delay, message));
        }
    }
}
