using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70013")]//堕落的佛兰明妮卡
    public class CorruptedFlaminca : CardEffect
    {// Discard a rain from your deck and spawn rain on its row an the opposite, if there is no rain in the deck, apply rain on both sides and damage self by 4
        public CorruptedFlaminca(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

        var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == CardId.TorrentialRain).Mess(Game.RNG);
        if (list.Count() == 0) // if there is no rain in the deck
            {
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
                await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
                await Card.Effect.Weaken(4, Card);
                return 0;
            }
        // rain in deck
        var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");
        await result.First().Effect.Discard(Card);
        await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
        await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
        return 0;
    
        }
    }
}
