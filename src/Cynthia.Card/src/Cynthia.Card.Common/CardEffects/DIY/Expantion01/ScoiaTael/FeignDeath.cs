using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70054")]//假死 FeignDeath
    public class FeignDeath : CardEffect
    {//复活2个战力高于4点的铜色单位，并对它们各造成4点伤害。
        public FeignDeath(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper 
            && x.CardInfo().CardType == CardType.Unit 
            && x.Status.Strength > 4).ToList();
            //让玩家选择
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, isCanOver: true);
            foreach (var x in result.ToList())
            {
                await x.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
                await x.Effect.Damage(4, Card);
            }
            return result.Count();
        }
    }
}