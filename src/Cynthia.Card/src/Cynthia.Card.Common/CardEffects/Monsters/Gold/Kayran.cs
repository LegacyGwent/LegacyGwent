using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22007")]//巨章鱼怪
    public class Kayran : CardEffect
    {//吞噬1个战力不高于7点的单位，将其战力传化为自身增益。
        public Kayran(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targetCards = await Game.GetSelectPlaceCards(Card, filter: x => x.CardPoint() <= 7, selectMode: SelectModeType.EnemyRow);

            if (!targetCards.TrySingle(out var target))
            {
                return 0;
            }

            await Card.Effect.Consume(target);
            return 0;
        }
    }
}