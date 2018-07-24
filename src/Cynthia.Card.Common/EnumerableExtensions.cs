using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;

namespace Cynthia.Algorithms
{
    public static class EnumerableExtensions
    {
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