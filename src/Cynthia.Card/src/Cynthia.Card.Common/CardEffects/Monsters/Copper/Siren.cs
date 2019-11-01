using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24033")]//女海妖
    public class Siren : CardEffect
    {//从牌组打出“月光”。
        public Siren(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targetCards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == CardId.Moonlight).ToList();
            if (targetCards.Count == 0)
            {
                return 0;
            }
            await targetCards.First().MoveToCardStayFirst();
            return 1;
        }
    }
}