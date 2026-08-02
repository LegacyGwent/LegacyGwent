using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("41002")]//雅妲公主
    public class PrincessAdda : CardEffect
    {//生成1个铜色北方领域“诅咒生物”单位。
        public PrincessAdda(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var ids = GwentMap.GetGenerateCardsId(
                x => x.Is(Group.Copper, CardType.Unit) &&
                    x.Faction == Faction.NorthernRealms &&
                    x.HasAllCategorie(Categorie.Cursed));
            return await Game.CreateAndMoveStay(PlayerIndex, ids.ToArray());
        }
    }
}
