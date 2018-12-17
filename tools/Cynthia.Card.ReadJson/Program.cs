using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cynthia.Card.ReadJson
{
    class Program
    {
        static void Main(string[] args)
        {
            var cardCategories = Readjson("E://昆特效果/Json/categories.json","zh-CN","en-US");
            foreach(var item in cardCategories)
            {
                Console.WriteLine($"key:{item.Key},value:{item.Value}");
            }

            Console.ReadLine();
        }
        public static IDictionary<string,string> Readjson(string path,string keyName,string valueName)
        {
            var result = new Dictionary<string,string>();
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject json = (JObject)JToken.ReadFrom(reader);
                    foreach(var item in json)
                    {
                        result[item.Value[keyName].ToString()] = item.Value[valueName].ToString();
                    }
                }
            }
            return result;
        }
    }
}
