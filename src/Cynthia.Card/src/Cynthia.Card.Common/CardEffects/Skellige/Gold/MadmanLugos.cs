using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62008")]//“疯子”卢戈
    public class MadmanLugos : CardEffect
    {//从牌组丢弃1张铜色单位牌，对1个敌军单位造成等同于被丢弃单位基础战力的伤害。
        public MadmanLugos(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于：先伤害 再丢弃 比较方便结算
            //乱序列出牌库中铜色单位
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Group != Group.Gold && x.CardInfo().CardUseInfo == CardUseInfo.MyRow).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");

            if (result.Count() == 0)
            {
                return 0;
            }

            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var discardtarget = result.First();
            await target.Effect.Damage(discardtarget.Status.Strength, Card);

            await discardtarget.Effect.Discard(Card);



            return 0;
        }
        private bool NotSliverSpy(GameCard gameCard)
        {
            return gameCard.Status.Group == Group.Silver && gameCard.CardInfo().CardType == CardType.Unit &&
                   gameCard.CardInfo().CardUseInfo == CardUseInfo.MyRow;
        }

    }
}


// public override async Task CardDownEffect(bool isSpying, bool isReveal)
// {
//     if (_target == null)
//     {
//         return;
//     }
//     await Consume(_target, x => x.Status.Strength);
// }        private GameCard discardtarget;
