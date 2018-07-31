using System.Collections.Generic;

namespace Cynthia.Card
{
    public class UserInfo : ModelBase
    {
        public string PlayerName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public IList<IEnumerable<string>> Decks { get; set; }
    }
}