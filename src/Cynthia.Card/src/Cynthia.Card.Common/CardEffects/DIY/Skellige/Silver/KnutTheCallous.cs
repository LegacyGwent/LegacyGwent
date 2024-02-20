using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70080")]//“无情者”克努特 KnuttheCallous
    public class KnuttheCallous : CardEffect
    {//
        public KnuttheCallous(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow && x.Status.Group == Group.Copper && x.HasAnyCategorie(Categorie.Soldier) && (x.CardInfo().CardType == CardType.Unit));
            if (list.Count() == 0)
            {
                return 0;
            }
            var StrengthList = list.Select(x => (Strength: x.Status.Strength, card: x)).OrderByDescending(x => x.Strength);
            var StrengthMaximun = StrengthList.First().Strength;
            var result = StrengthList.Where(x => x.Strength >= StrengthMaximun).Select(x => x.card);
            if (!result.TryMessOne(out var target, Game.RNG))
            {
                return 0;
            }
            var result1 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (result1.Count == 0) return 0;
           
            await result1.Single().Effect.Damage(target.CardPoint()/2, Card);
            await target.Effect.Damage(target.CardPoint()/2, Card);
            await target.MoveToCardStayFirst();
            return 1;
        }
    }
}
