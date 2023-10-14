using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("89007")]//铁隼飞刀手 IronFalconKnifeJuggler
    public class IronFalconKnifeJuggler : CardEffect
    {//生成1个自身相同战力的佚亡复制，并使场上的同名牌获得1点增益。
        public IronFalconKnifeJuggler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var position = Card.GetLocation();
            var cards = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.Status.CardId == Card.Status.CardId).ToList();
            await Game.CreateCard(Card.Status.CardId, PlayerIndex, position, setting: ToDoomed);
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, Card);
            }
            return 0;
        }

        private void ToDoomed(CardStatus status)
        {
            status.IsDoomed = true;
            status.Strength = Card.CardPoint();
        }
    }


}
