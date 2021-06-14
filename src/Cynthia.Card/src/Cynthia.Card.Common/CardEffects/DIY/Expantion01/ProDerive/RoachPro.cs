using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130010")]//萝卜：晋升
    public class RoachPro : CardEffect, IHandlesEvent<AfterUnitDown>
    {//己方从手牌打出金色单位牌时，召唤此单位。若位于手牌，打出至随机排，然后抽1张牌。若位于墓地，复活至随机位置。
        public RoachPro(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            //在牌组
            if (@event.Target.Status.Group == Group.Gold && (@event.Target.PlayerIndex == Card.PlayerIndex || (@event.IsSpying == true && @event.Target.PlayerIndex != Card.PlayerIndex)) && Card.Status.CardRow.IsInDeck() && @event.IsFromHand && !@event.IsSpying)
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.Target);
            }
            //在手牌
            else if (@event.Target.Status.Group == Group.Gold && (@event.Target.PlayerIndex == Card.PlayerIndex || (@event.IsSpying == true && @event.Target.PlayerIndex != Card.PlayerIndex)) && Card.Status.CardRow.IsInHand() && @event.IsFromHand && !@event.IsSpying)
            {
                var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
                await Card.Effect.Summon(location, @event.Target);
                await Game.PlayerDrawCard(Card.PlayerIndex, 1);
            }
            //在墓地
            else if (@event.Target.Status.Group == Group.Gold && (@event.Target.PlayerIndex == Card.PlayerIndex || (@event.IsSpying == true && @event.Target.PlayerIndex != Card.PlayerIndex)) && Card.Status.CardRow.IsInCemetery() && @event.IsFromHand && !@event.IsSpying)
            {
                await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            }
        }

    }
}