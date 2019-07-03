using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32014")]//古雷特的雷索
    public class LethoOfGulet : CardEffect
    {//间谍。改变同排2个单位的锁定状态，随后汲食它们的所有战力。
        public LethoOfGulet(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 2, true, x => x.Status.CardRow == Card.Status.CardRow, SelectModeType.MyRow);
            foreach(var card in cards)
            {
                await card.Effect.Lock(Card);
                await Card.Effect.Drain(card.ToHealth().health, card);
            }
            return 0;
        }
    }
}