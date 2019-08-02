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
            var target = GwentMap.GetCards().Where(x => x.Is(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.ClanTuirseach, Categorie.Soldier)).Select(x => x.CardId));
            //创造,如果没选择,什么都不做
            var count = (await Game.CreateAndMoveStay(PlayerIndex, target.ToArray()));
            if (count == 0)
            {
                return 0;
            }
        }
    }
}