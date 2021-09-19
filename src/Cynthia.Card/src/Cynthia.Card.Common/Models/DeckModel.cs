using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class DeckModel : ModelBase
    {
        public string Name { get; set; } = "";
        public List<string> Deck { get; set; } = new List<string>();
        public string Leader { get; set; } = "";
        public int IsSpecial { get; set; } = 0;
    }

}