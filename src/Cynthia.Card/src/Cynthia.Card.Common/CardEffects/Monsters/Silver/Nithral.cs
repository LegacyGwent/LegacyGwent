using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23011")]//尼斯里拉
    public class Nithral : CardEffect
    {//对1个敌军单位造成6点伤害。手牌中每有1张“狂猎”单位牌，伤害提高1点。
        public Nithral(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var point = Game.PlayersHandCard[PlayerIndex].Where(x => x.HasAllCategorie(Categorie.WildHunt)).Count();
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(6 + point, Card);
            return 0;
        }
    }
}