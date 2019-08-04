using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63007")]//多纳·印达
    public class Donar : CardEffect
    {//改变1个单位的锁定状态。从对方墓场中1张铜色单位牌移至己方墓场。
        public Donar(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择一个锁定单位
            var LockTarget = await Game.GetSelectPlaceCards(Card);
            if (!(LockTarget.Count() == 0))
            {
                await LockTarget.Single().Effect.Lock(Card);
            }
            //列出敌方墓地铜色单位
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