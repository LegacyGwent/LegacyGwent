using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("61003")]//埃斯特·图尔赛克
    public class EistTuirseach : CardEffect
    {//生成1个铜色“图尔赛克家族”的“士兵”单位。
        public EistTuirseach(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {   //图尔赛克家族，士兵，铜单位
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.ClanTuirseach)).Select(x => x.CardId);
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
        }
    }
}