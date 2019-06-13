using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14008")]//瘟疫
    public class Epidemic : CardEffect
    {//摧毁所有最弱的单位。
        public Epidemic(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x => x.Status.CardRow.IsOnPlace()).WhereAllLowest().ToList();
            for (var i = 0; i < cards.Count; i++)
            {
                await cards[i].Effect.ToCemetery(CardBreakEffectType.Epidemic);
            }
            // foreach (var card in cards)
            // {
            //     await card.Effect.ToCemetery(CardBreakEffectType.Epidemic);
            // }
            return 0;
        }
    }
}