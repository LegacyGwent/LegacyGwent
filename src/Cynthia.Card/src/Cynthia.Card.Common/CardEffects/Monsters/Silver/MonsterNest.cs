using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23021")]//怪物巢穴
    public class MonsterNest : CardEffect
    {//生成1个铜色“食腐生物”或“类虫生物”单位，使其获得1点增益。
        public MonsterNest(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var target = GwentMap.GetCards()
                .Where(x => x.Is(Group.Copper, CardType.Unit, x => x.HasAnyCategorie(Categorie.Necrophage, Categorie.Insectoid)))
                .Select(x => x.CardId);
            var count = (await Game.CreateAndMoveStay(PlayerIndex, target.ToArray()));
            if (count == 0)
            {
                return 0;
            }
            return 0;
        }
    }
}