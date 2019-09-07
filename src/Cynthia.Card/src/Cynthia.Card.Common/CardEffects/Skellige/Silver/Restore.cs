using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63019")]//回复
    public class Restore : CardEffect
    {//将墓场1张铜色/银色“史凯利格”单位牌置入手牌，为其添加佚亡标签，再将其基础战力设为8点，随后打出1张牌。
        public Restore(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //列出墓地的铜色/银色“史凯利格”单位牌,如果没有,什么都不做
            var Clist = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit && x.Status.Faction == Faction.Skellige);
            if (Clist.Count() == 0)
            {
                return 0;
            }

            //选择一张，如果没有选，结束
            var Cresult = await Game.GetSelectMenuCards(Card.PlayerIndex, Clist.ToList(), 1);
            if (Cresult.Count() == 0)
            {
                return 0;
            }
            //回手
            Cresult.Single().Effect.Repair(true);
            int offset = 8 - Cresult.Single().Status.Strength;
            if (offset > 0)
                await Cresult.Single().Effect.Strengthen(offset, Card);
            else if (offset < 0)
                await Cresult.Single().Effect.Weaken(-offset, Card);

            //设置佚亡，回手
            Cresult.Single().Status.IsDoomed = true;
            await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyHand, CardIndex = 0 }, Cresult.Single(), refreshPoint: true);

            //从手牌选择一张打出，必须打
            var Hlist = Game.PlayersHandCard[Card.PlayerIndex];

            var Hresult = await Game.GetSelectMenuCards(Card.PlayerIndex, Hlist.ToList(), 1, isCanOver: false);

            await Hresult.Single().MoveToCardStayFirst();

            return 1;
        }
    }
}