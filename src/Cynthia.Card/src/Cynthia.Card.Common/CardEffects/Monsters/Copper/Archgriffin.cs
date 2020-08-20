using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24004")]//大狮鹫
	public class Archgriffin : CardEffect
	{//移除所在排的灾厄。
		public Archgriffin(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            var enemylist = Game.PlayersCemetery[Game.AnotherPlayer(Card.PlayerIndex)].Where(x => x.CardInfo().CardType == CardType.Unit && x.Status.Group == Group.Copper).ToList();
            //如果没有铜色单位，什么都不做
            if (enemylist.Count() == 0)
            {
                return 0;
            }
            //选择一个铜色单位，如果不选，什么都不做
            var MoveTarget = await Game.GetSelectMenuCards(Card.PlayerIndex, enemylist.ToList(), 1);
            if (MoveTarget.Count() == 0)
            {
                return 0;
            }
            //移动到我方墓地
            //不清楚refresh的含义
            await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.EnemyCemetery, CardIndex = 0 }, MoveTarget.Single());
            return 0;
		}
	}
}