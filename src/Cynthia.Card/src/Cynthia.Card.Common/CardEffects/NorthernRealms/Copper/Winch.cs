using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44033")]//绞盘
    public class Winch : CardEffect
    {//使所有己方半场的“机械”单位获得3点增益。
        public Winch(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Machine) && x.CardInfo().CardType == CardType.Unit);
            foreach (var card in list)
            {
                await card.Effect.Boost(3, Card);
            }
            return 0;
        }
    }
}