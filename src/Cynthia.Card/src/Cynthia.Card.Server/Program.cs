using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System.IO;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // if (Directory.Exists("./logs")) Directory.CreateDirectory("./logs");
            // var name = DateTime.UtcNow.ToString("s").Replace(":", "");
            // var sw = new StreamWriter(new FileStream($"./{name}log.log", FileMode.Create));
            // var se = new StreamWriter(new FileStream($"./{name}Elog.log", FileMode.Create));
            // Console.SetOut(sw);
            // Console.SetError(se);
            // try
            // {
            // var updateTask = TimingUpdate(20, sw, se);
            Command.MongodbConnect();
            CreateHostBuilder(args).Build().Run();
            // }
            // finally
            // {
            //     sw.Close();
            //     se.Close();
            // }
        }

        public static async Task TimingUpdate(int updateTime, StreamWriter sw, StreamWriter se)
        {
            while (true)
            {
                await Task.Delay(1000 * updateTime);
                sw.Flush();
                se.Flush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                .UseNLog()
                .UseUrls("http://*:5005");
            });
    }
}