using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34003")]//近卫军
    public class ImperaBrigade : CardEffect, IHandlesEvent<AfterCardSpying>
    {
        public ImperaBrigade(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //对方半场的间谍数量
            var count = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.PlayerIndex == Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.Status.CardRow.IsOnPlace() && x.Status.IsSpying).Count();
            //每有一个间谍获得2点增益
            await Boost(count * 2);
            return 0;
        }

        public async Task HandleEvent(AfterCardSpying @event)
        {
            //如果在场上, 并且对方半场出现间谍, 增益2点
            if (Card.Status.CardRow.IsOnPlace() && @event.Target.PlayerIndex != Card.PlayerIndex)
                await Boost(2);
        }
    }
}