using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70002")]//考德威尔伯爵
    public class DetlaffHigherVampire : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardHurt>
    {//择一：从牌库中打出一张战力不高于自身的铜色单位，在回合结束把它送进墓地；或吞噬牌库中一张战力高于自身的铜色单位牌，将它的战力作为自身的增益。
        public DetlaffHigherVampire(GameCard card) : base(card) { }

        private bool _needKill = false;
        private GameCard _target = null;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //look at bronzes, if there are no bronzes stop
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Is(Group.Copper, CardType.Unit)).Mess(Game.RNG).ToList();
            if (list.Count() == 0) { return 0; }
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0) { return 0; }
            // if points lower than self destroy target
            _target = cards.Single();
            if (_target.CardPoint() <= Card.CardPoint()) 

            {
            //need to kill the target
                _needKill = true;

                await _target.MoveToCardStayFirst();
                return 1;
            }
            // consume when power bigger than self
            else if (_target.CardPoint() > Card.CardPoint()) 
            {
                await Card.Effect.Consume(_target);
                return 0;
            }
            return 0;
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (_needKill && Card.Status.CardRow.IsOnPlace())
            {
                //如果要摧毁的单位已经死亡
                if (@event.Target == _target && @event.Target.IsDead)
                {
                    _needKill = false;
                }
            }
            await Task.CompletedTask;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (_needKill && Card.Status.CardRow.IsOnPlace())
            {
                if (_target.Status.CardRow.IsOnPlace())
                {
                    await Game.Debug("伯爵送入墓地");
                    await _target.Effect.ToCemetery(CardBreakEffectType.Scorch);
                    await Game.Debug("伯爵送入墓地完成");

                }
                // modifications to accomodate plumard's effect
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == "70147").ToList();
                if (list.Count() > 0)
                {
                    var plumard = list.Last();
                    await plumard.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), plumard);
                }
                // end of plumard section
                _needKill = false;
            }
        }
    }
}