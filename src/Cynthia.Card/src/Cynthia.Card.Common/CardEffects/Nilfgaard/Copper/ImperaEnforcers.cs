using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34005")]//近卫军铁卫
    public class ImperaEnforcers : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardSpying>
    {
        public ImperaEnforcers(GameCard card) : base(card) { }
        public int SpyingCount { get; set; } = 0;
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //选择对方场上一张卡,如果目标不为空,对其造成2点伤害
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(2, Card);

            var count = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.PlayerIndex == Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.Status.CardRow.IsOnPlace() && x.Status.IsSpying).Count();
            for (var i = 0; i < count-SpyingCount; i++)
                {   //重复计数次效果,之后清空计数
                    var results = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
                    if (results.Count != 0) await results.Single().Effect.Damage(2, Card);
                }     
            return 0;
        }

        public Task HandleEvent(AfterCardSpying @event)
        {
            //当对方场上出现间谍,并且自己在场,计数+1
            if (@event.Target.PlayerIndex != Card.PlayerIndex)
                SpyingCount++;
            return Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            //在回合结束,触发效果
            if (Card.PlayerIndex == @event.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                for (var i = 0; i < SpyingCount; i++)
                {   //重复计数次效果,之后清空计数
                    var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
                    if (result.Count != 0) await result.Single().Effect.Damage(2, Card);
                }
            }
            SpyingCount = 0;
        }
    }
}