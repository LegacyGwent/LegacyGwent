using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70013")]//堕落的佛兰明妮卡
    public class CorruptedFlaminca : CardEffect
    {// Deploy: apply rain on both sides
        public CorruptedFlaminca(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
            await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<TorrentialRainStatus>();
            return 0;
        }
    }
}
