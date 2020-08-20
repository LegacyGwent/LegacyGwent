using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34024")]//阿尔巴师枪兵  
    public class AlbaPikeman : CardEffect, IHandlesEvent<AfterTurnStart>
    {//回合开始时从卡组召唤1张同名牌，2点护甲。
        public AlbaPikeman(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }

        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(1);
            return;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == @event.PlayerIndex))
                return;
            if (Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == Card.Status.CardId))
            {
                if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).TryMessOne(out var card, Game.RNG) || Card.Status.Countdown < 1)
                {
                    return;
                }
                await Card.Effect.SetCountdown(offset: -1);
                await card.Effect.Summon(Card.GetLocation() + 1, Card);
                await card.Effect.SetCountdown(1);
                await card.Effect.Armor(2, Card);
                //一定程度上是强行提供了特例护甲，“召唤”按道理不应当带护甲
                //但蠢驴其中一个版本就是这样
                //我本人也比较支持护甲是基本属性
            }
            return;
        }
    }
}