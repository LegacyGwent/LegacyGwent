using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions.Extensions;
using Newtonsoft.Json;

namespace Cynthia.Card
{
    public static class EnumerableExtensions
    {
        public static bool TrySingle<T>(this IEnumerable<T> items, out T result)
        {
            var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                result = default;
                return false;
            }
            result = enumerator.Current;
            if (enumerator.MoveNext())
            {
                result = default;
                return false;
            }
            return true;
        }

        public static bool TryMessOne<T>(this IEnumerable<T> items, out T result, Random rng)
        {
            result = items.Mess(rng).FirstOrDefault();
            return result == default ? false : true;
        }
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
            var a = typeof(T).Name;
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public static IEnumerable<T> Mess<T>(this IEnumerable<T> source, Random rng)
        {
            var arr = source.ToArray();
            var pickeds = false.Plural(arr.Length).ToArray();
            var num = 0;
            while (num < arr.Length)
            {
                var r = rng.Next(arr.Length - num);
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