using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("31002")]//恩希尔·恩瑞斯
    public class EmhyrVarEmreis : CardEffect
    {//打出1张手牌，随后将1个友军铜色/银色单位收回手牌。
        public EmhyrVarEmreis(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = Game.PlayersHandCard[Card.PlayerIndex];
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, isCanOver: false);
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();
            return 1;
        }
        public override async Task CardDownEffect(bool isSpying)
        {
            var result = await Game.GetSelectPlaceCards(Card, Sizer: (x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver), selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return;
            result.Single().Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyHand, CardIndex = 0 }, result.Single(), refreshPoint: true);
        }
    }
}