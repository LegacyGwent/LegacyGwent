using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44025")]//中邪的女术士
    public class DamnedSorceress : CardEffect
    {//若同排有1个“诅咒生物”单位，则造成7点伤害。
        public DamnedSorceress(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            var list = Game.RowToList(PlayerIndex, Card.Status.CardRow).Where(x => x.HasAnyCategorie(Categorie.Cursed) && x != Card);
            if (list.Count() >= 1)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
                if (!selectList.TrySingle(out var target))
                {
                    return;
                }
                await target.Effect.Damage(7, Card);
            }
            return;
        }
    }
}