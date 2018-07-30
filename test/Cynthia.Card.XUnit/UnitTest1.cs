using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card.XUnit
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var list = new List<int>();
            list.Add(10);
            list.Add(15);
            list.Add(20);
            list.Add(25);
            var result = 0;
            result = list.First(x => x > 10);
            return;
        }
    }
}
