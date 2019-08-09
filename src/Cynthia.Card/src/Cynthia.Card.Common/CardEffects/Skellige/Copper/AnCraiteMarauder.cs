using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64020")]//奎特家族劫掠者
    public class AnCraiteMarauder : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<AfterTurnOver>
    {//造成4点伤害。若被复活，则造成6点伤害。
        public AnCraiteMarauder(GameCard card) : base(card) { }
        private bool _resurrectedflag = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)

        {

            //选取一个任意单位
            int damagepoint = _resurrectedflag ? 6 : 4;
            _resurrectedflag = false;
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(damagepoint, Card);

            return 0;
        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            //进入墓地的不是本卡，什么都不发生
            if (@event.Target != Card)
            {
                return;
            }
            //去过墓地之后，使标签为true
            _resurrectedflag = true;
            await Task.CompletedTask;
        }

        //每回合结束排除从墓地回牌组的情况
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (Card.Status.CardRow == RowPosition.MyDeck)
            {
                _resurrectedflag = false;
            }
            await Task.CompletedTask;
        }
    }
}