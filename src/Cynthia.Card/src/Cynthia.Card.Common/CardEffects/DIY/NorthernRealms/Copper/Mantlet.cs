using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70130")]//防盾 Mantlet
    public class Mantlet : CardEffect
    {//若己方牌组没有“防盾”则将1张“防盾”加入牌组，驱动：获得4点护甲。
        public Mantlet(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            var targetId = Card.Status.CardId;
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            if (cards.Count == 0)
            {
                await Game.CreateCardAtEnd(targetId, PlayerIndex, RowPosition.MyDeck);
            }
            
            return;
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Card.Status.CardRow.IsOnPlace())
            {
                for (var i = 0; i < Card.GetCrewedCount(); i++)
                {
                    if (i == 0)
                    {
                        await Card.Effect.Armor(6, Card);
                        return 0;
                    }
                    
                }
            }
            return 0;
        }
    }
}
