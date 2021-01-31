using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

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
            Console.WriteLine(Directory.GetCurrentDirectory());
            Command.MongodbConnect();
            //从数据库载入默认图片素材
            Command.InItDefaultTexture();
            //第一次初始化卡牌数据时使用
            //Command.InItDefaultTexture(new DefaultTexture()
            //{
            //    cardLoadImage = File.ReadAllBytes("卡画.jpg"),
            //    cardUploadImage = File.ReadAllBytes("卡画.jpg"),
            //    cardFramesImage = File.ReadAllBytes("卡框.png"),
            //}) ;

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
                .UseUrls("http://*:5000");
            });
    }
}