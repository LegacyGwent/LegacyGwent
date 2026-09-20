using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

partial class Program
{
    static async Task<int> RunProgram(string exe,IEnumerable<string> arguments,string output,IDictionary<string,string> environment=null)
    {
        var start=new ProcessStartInfo(exe){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
        foreach(var arg in arguments)start.ArgumentList.Add(arg);
        if(environment!=null)foreach(var pair in environment)start.Environment[pair.Key]=pair.Value;
        using(var process=Process.Start(start))
        {
            var stdout=process.StandardOutput.ReadToEndAsync();var stderr=process.StandardError.ReadToEndAsync();
            if(!await Task.Run(()=>process.WaitForExit(90000))){process.Kill();throw new TimeoutException("Regression process: "+Path.GetFileName(exe));}
            File.WriteAllText(output,await stdout+Environment.NewLine+await stderr);return process.ExitCode;
        }
    }
    static async Task ExternalRegressions()
    {
        var mongoUri=Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121";
        var mongoDatabaseUri=mongoUri.TrimEnd('/');
        if(!mongoDatabaseUri.EndsWith("/gwentdiy",StringComparison.OrdinalIgnoreCase))mongoDatabaseUri+="/gwentdiy";
        var dotnet=Environment.ProcessPath ?? "dotnet";
        var outcomes=new List<object>();
        foreach(var test in new[]{"DailyQuestTest","PremiumCraftingTest","RewardClientTest"})
        {
            var output=Path.Combine(work,test+".log");
            var args=new List<string>{Path.Combine(root,"src/Cynthia.Card/test",test,"bin/Release/net10.0",test+".dll")};
            if(test=="RewardClientTest")args.Add(Path.Combine(work,"client-results.json"));
            var code=await RunProgram(dotnet,args,output,new Dictionary<string,string>{{"REWARD_TEST_MONGO_URI",mongoUri},{"DOTNET_MULTILEVEL_LOOKUP","0"}});
            outcomes.Add(new{test,exitCode=code,log=output});Check(code==0,test+" standalone regression succeeds",new{exitCode=code,log=output});
        }
        db.DailyQuestClock=()=>DateTimeOffset.UtcNow;
        try
        {
            foreach(var test in new[]{"Test-DailyQuestsHub","Test-PremiumCraftingHub"})
            {
                var args=new List<string>{"-NoProfile","-NonInteractive","-ExecutionPolicy","Bypass","-File",Path.Combine(root,"scripts",test+".ps1"),"-Endpoint",url,"-OutputPath",Path.Combine(work,test+".json")};
                if(test=="Test-PremiumCraftingHub")args.AddRange(new[]{"-MongoUri",mongoDatabaseUri});
                var output=Path.Combine(work,test+".log");var code=await RunProgram("powershell.exe",args,output);
                outcomes.Add(new{test,exitCode=code,log=output});Check(code==0,test+" network regression succeeds",new{exitCode=code,log=output});
            }
        }
        finally{db.DailyQuestClock=()=>Now;File.WriteAllText(Path.Combine(work,"external-regressions.json"),JsonConvert.SerializeObject(outcomes,Formatting.Indented));}
    }
    static async Task ConfigProbe(string output)
    {
        var mongoUri=Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121";
        var provider=new ServiceCollection().AddSingleton<IMongoClient>(new MongoClient(mongoUri)).AddSingleton(new InitialPowderOptions(0)).BuildServiceProvider();
        var service=new GwentDatabaseService(provider);var user=new UserInfo{UserName="reward-config-"+Guid.NewGuid().ToString("N"),PlayerName="config",Decks=new List<DeckModel>()};
        await service.GetMongoClient().GetDatabase("gwentdiy").GetCollection<UserInfo>("user").InsertOneAsync(user);
        try
        {
            var state=await service.GetDailyQuests(user.UserName);
            File.WriteAllText(output,JsonConvert.SerializeObject(new{success=state.Success,cap=state.DailyCap,balance=state.Wallet.Collection.MeteoritePowder}));
        }
        catch(Exception e)
        {
            var wallet=await service.GetMongoClient().GetDatabase("gwentdiy").GetCollection<PremiumCollection>("premium_collection").Find(x=>x.Id==user.Id).FirstOrDefaultAsync();
            File.WriteAllText(output,JsonConvert.SerializeObject(new{success=false,error=e.GetType().Name,message=e.Message,balance=wallet?.MeteoritePowder??0}));
        }
    }
    static async Task ConfigurationCases()
    {
        var folder=Path.Combine(work,"config-probe");Directory.CreateDirectory(folder);
        foreach(var source in Directory.GetFiles(AppContext.BaseDirectory))File.Copy(source,Path.Combine(folder,Path.GetFileName(source)),true);
        var cases=new[]{
            ("missing",(string)null,false),("malformed","{",false),("null","null",false),
            ("empty","{\"LoginPowder\":10,\"Tiers\":[]}",false),
            ("zero","{\"LoginPowder\":0,\"Tiers\":[{\"Crowns\":2,\"Powder\":10}]}",false),
            ("negative","{\"LoginPowder\":10,\"Tiers\":[{\"Crowns\":2,\"Powder\":-1}]}",false),
            ("duplicate","{\"LoginPowder\":10,\"Tiers\":[{\"Crowns\":2,\"Powder\":10},{\"Crowns\":2,\"Powder\":20}]}",false),
            ("unordered","{\"LoginPowder\":10,\"Tiers\":[{\"Crowns\":4,\"Powder\":10},{\"Crowns\":2,\"Powder\":20}]}",false),
            ("overflow","{\"LoginPowder\":2147483647,\"Tiers\":[{\"Crowns\":2,\"Powder\":10}]}",false),
            ("null-tier","{\"LoginPowder\":10,\"Tiers\":[null]}",false),
            ("valid","{\"LoginPowder\":10,\"Tiers\":[{\"Crowns\":2,\"Powder\":10},{\"Crowns\":4,\"Powder\":20},{\"Crowns\":6,\"Powder\":30}]}",true)
        };
        foreach(var item in cases)
        {
            var config=Path.Combine(folder,"DailyQuests.json");if(File.Exists(config))File.Delete(config);
            if(item.Item2!=null)File.WriteAllText(config,item.Item2);
            var output=Path.Combine(folder,item.Item1+".json");
            int code=await RunProgram(Environment.ProcessPath ?? "dotnet",
                new[]{Path.Combine(folder,"RewardSystemTest.dll"),"--config-probe",output},Path.Combine(folder,item.Item1+".log"));
            var state=JObject.Parse(File.ReadAllText(output));
            Check(code==0 && (bool)state["success"]==item.Item3,"configuration "+item.Item1+" validates before reward response",state);
            if(!item.Item3)Check((long)state["balance"]==0,"invalid configuration "+item.Item1+" never grants powder",state);
        }
    }
}
