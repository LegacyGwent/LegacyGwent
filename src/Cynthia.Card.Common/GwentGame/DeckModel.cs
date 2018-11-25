using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class DeckModel
    {
        [Key]
        public string Id{get;set;}
        public string Name { get; set; }
        public List<string> Deck { get; set; }
        public string Leader { get; set; }
    }
}