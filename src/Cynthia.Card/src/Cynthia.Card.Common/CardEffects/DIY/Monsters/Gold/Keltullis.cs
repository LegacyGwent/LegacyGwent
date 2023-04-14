using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70113")]//克尔图里斯
    public class Keltullis : CardEffect, IHandlesEvent<AfterTurnOver>
    {//6护甲，回合结束时随机摧毁1个场上战力最低且低于自身基础战力的单位。
        public Keltullis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Armor(2, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if (!Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).FilterCards(filter: x => x.CardPoint() < 5 && x != Card).Where(x => x.Status.CardRow.IsOnPlace()).WhereAllLowest().TryMessOne(out var target, Game.RNG))
            {
                return;
            }

            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
        }
    }
}
