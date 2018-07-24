using System;
using Xunit;
using Cynthia.Card.Server;
using Cynthia.Card.Common;
using Cynthia.Card.Client;

namespace Cynthia.Card.XUnit
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var deck = new GwentDeck();
        }
    }
}
