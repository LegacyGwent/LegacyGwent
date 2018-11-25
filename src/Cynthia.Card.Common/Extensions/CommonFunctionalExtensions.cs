using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class CommonFunctionalExtensions
    {
        public static IEnumerable<(T,int)> Indexed<T>(this IEnumerable<T> source)
        {
            return source.Select((item,index)=>(item,index));
        }
    }
}