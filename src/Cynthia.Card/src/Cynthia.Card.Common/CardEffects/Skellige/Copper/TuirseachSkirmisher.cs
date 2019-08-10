using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64012")]//图尔赛克家族好斗分子
    public class TuirseachSkirmisher : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<AfterTurnOver>
    {//被复活后获得3点强化。
        public TuirseachSkirmisher(GameCard card) : base(card) { }
        private bool _resurrectedflag = false;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)

        {
            if (_resurrectedflag)
            {
                _resurrectedflag = false;
                await Card.Effect.Strengthen(3, Card);
            }
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