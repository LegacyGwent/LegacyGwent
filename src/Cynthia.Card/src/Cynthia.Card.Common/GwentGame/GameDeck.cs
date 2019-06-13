using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class GameDeck
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<GwentCard> Deck { get; set; }
        public GwentCard Leader { get; set; }
    }
}