using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64024")]//奎特家族突袭者
    public class AnCraiteRaider : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//被丢弃时复活自身。
        public AnCraiteRaider(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDiscard @event)
        {
            //进入墓地的不是本卡，什么都不发生
            if (@event.Target != Card)
            {
                return;
            }
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
        }
    }
}