using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44011")]//攻城后援
    public class SiegeSupport : CardEffect, IHandlesEvent<AfterUnitDown>
    {//使后续打出的友军单位获得1点增益，“机械”单位额外获得1点护甲。 操控。
        public SiegeSupport(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterUnitDown @event)
        {
            //以下代码基于 打入我方半场的间谍单位也会被buff
            if (@event.Target.PlayerIndex == Card.PlayerIndex && Card.GetLocation().RowPosition.IsOnPlace() && @event.Target != Card)
            {
                await @event.Target.Effect.Boost(1, Card);
                if (@event.Target.HasAnyCategorie(Categorie.Machine))
                {
                    await @event.Target.Effect.Armor(1, Card);
                }

            }
            return;
        }

    }
}