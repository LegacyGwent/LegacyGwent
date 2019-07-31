using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64029")]//海玫家族女矛手
    public class HeymaeySpearmaiden : CardEffect
    {//对1个友军“机械”或“士兵”单位造成1点伤害，随后从牌组打出1张它的同名牌。
        public HeymaeySpearmaiden(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, false, x => x.Is(Group.Copper, CardType.Unit, x =>(x.HasAllCategorie(Categorie.Soldier) || x.HasAllCategorie(Categorie.Machine)) && Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == x.Status.CardId)), SelectModeType.MyRow);

            if (!targets.TrySingle(out var target))
            {
                return 0;
            }
            if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == target.Status.CardId).TryMessOne(out var card, Game.RNG))
            {
                return 0;
            }
			await target.Effect.Damage(1,Card);
            await card.MoveToCardStayFirst();
            return 1;
        }
    }
}