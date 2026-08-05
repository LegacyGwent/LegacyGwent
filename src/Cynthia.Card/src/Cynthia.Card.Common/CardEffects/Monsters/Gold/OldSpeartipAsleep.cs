using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22001")]//老矛头：昏睡
    public class OldSpeartipAsleep : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardTransform>
    {//使相邻单位获得1点增益，自身获得5点护甲。己方回合开始时，若对方同排有至少3个单位，则苏醒。
        public OldSpeartipAsleep(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ArmorAndBoost();
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }

            var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
            if (row.Count() >= 3)
            {
                await Card.Effect.Transform(CardId.OldSpeartip, Card);
            }
        }

        public async Task HandleEvent(AfterCardTransform @event)
        {
            if (@event.Target != Card || !Card.IsAliveOnPlance() ||
                @event.Source.Status.CardId != CardId.OldSpeartipAsleep)
            {
                return;
            }

            await ArmorAndBoost();
        }

        private async Task ArmorAndBoost()
        {
            await Card.Effect.Armor(5, Card);
            foreach (var adjacent in Card.GetRangeCard(1, GetRangeType.HollowAll)
                .Where(x => x.Status.CardRow.IsOnPlace()).ToList())
            {
                await adjacent.Effect.Boost(1, Card);
            }
        }
    }
}
