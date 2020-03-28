using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70013")]//堕落的佛兰明妮卡
    public class CorruptedFlaminca : CardEffect
    {//部署：弃掉牌库里一张倾盆大雨，然后在敌我双方同排降下倾盆大雨。
        public CorruptedFlaminca(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == CardId.TorrentialRain).Mess(Game.RNG);
            if (list.Count() == 0) { return 0; }

            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");

            if (result.Count() == 0) { return 0; }

            await result.First().Effect.Discard(Card);
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
            await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
            return 0;
        }
    }
}