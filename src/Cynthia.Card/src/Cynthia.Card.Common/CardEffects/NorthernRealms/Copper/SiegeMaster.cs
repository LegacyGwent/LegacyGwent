using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44019")]//攻城大师
    public class SiegeMaster : CardEffect
    {//治愈一个铜色/银色友军“机械”单位，并再次触发其能力。 操控。
        public SiegeMaster(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow, filter: x => x.HasAnyCategorie(Categorie.Machine) && (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            if (target.Status.HealthStatus < 0)
            {
                await target.Effect.Heal(Card);
            }
            if(target.Status.IsLock)
			{
				return 0;
			}
            else
            {
                return await target.Effect.CardPlayEffect(false, true);
            }
        }
    }
}