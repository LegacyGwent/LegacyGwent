using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70091")]//神灯 Magic Lamp
    public class MagicLamp : CardEffect, IHandlesEvent<OnGameStart>
    {//对局开始时，将3张“最后的愿望”加入卡组，随后丢弃自身。

        public MagicLamp(GameCard card) : base(card) { }
        
        public async Task HandleEvent(OnGameStart @event)
        {
            for (var i = 0; i < 3; i++)
            {
                await Game.CreateCard(CardId.TheLastWish, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            }
            await Card.Effect.Discard(Card);
            return;
        }
    }
}
