using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("41002")]//雅妲公主
    public class PrincessAdda : CardEffect
    {//创造1个铜色/银色“诅咒生物”单位。
        public PrincessAdda(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var ids = GwentMap.GetCreateCardsId(x => x.Is(filter: x => x.HasAllCategorie(Categorie.Cursed) && x.IsAnyGroup(Group.Copper, Group.Silver)), RNG);
            return await Game.CreateAndMoveStay(PlayerIndex, ids.ToArray());
        }
    }
}