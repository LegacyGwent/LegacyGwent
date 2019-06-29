using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class DeckModel : ModelBase
    {
        public string Name { get; set; }
        public List<string> Deck { get; set; }
        public string Leader { get; set; }
    }
}