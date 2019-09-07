using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44030")]//女巫猎人
    public class WitchHunter : CardEffect
    {//重置1个单位。若它为“法师”，则从牌组打出1张同名牌。
        public WitchHunter(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Reset(Card);
            if (target.HasAllCategorie(Categorie.Mage) && Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == Card.Status.CardId))
            {
                if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).TryMessOne(out var card, Game.RNG))
                {
                    return 0;
                }
                await card.MoveToCardStayFirst();
                return 1;
            }
            return 0;
        }

    }
}