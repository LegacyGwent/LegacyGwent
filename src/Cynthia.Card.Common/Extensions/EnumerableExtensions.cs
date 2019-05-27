using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions.Extensions;
using Newtonsoft.Json;

namespace Cynthia.Card
{
    public static class EnumerableExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static object ToObject(this string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }
        public static T ToType<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public static IEnumerable<T> Mess<T>(this IEnumerable<T> source)
        {
            var arr = source.ToArray();
            var pickeds = false.Plural(arr.Length).ToArray();
            var num = 0;
            var rand = new Random();
            while (num < arr.Length)
            {
                var r = rand.Next(arr.Length - num);
                var index = r;
                for (var i = 0; i <= index; i++)
                {
                    while (pickeds[i])
                    {
                        index++;
                        i++;
                    }
                }
                pickeds[index] = true;
                yield return arr[index];
                num++;
            }
        }
    }
}