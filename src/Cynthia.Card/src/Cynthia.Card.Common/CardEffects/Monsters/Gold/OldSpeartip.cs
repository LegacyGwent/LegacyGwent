using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22003")]//老矛头
    public class OldSpeartip : CardEffect, IHandlesEvent<AfterTurnStart>
    {//对最多5个敌军同排单位造成2点伤害。己方回合开始时若对方同排单位不足3个，则沉睡。
        public OldSpeartip(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await OldSpeartipRules.DamageOppositeRow(Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
            if (row.Count() < 3)
            {
                await Card.Effect.Transform(CardId.OldSpeartipAsleep, Card);
                await OldSpeartipRules.ArmorAndBoostAdjacent(Card);
            }
        }
    }

    internal static class OldSpeartipRules
    {
        internal static async Task DamageOppositeRow(GameCard source)
        {
            foreach (var target in source.Game
                .RowToList(source.PlayerIndex, source.Status.CardRow.Mirror())
                .IgnoreConcealAndDead().Mess(source.Game.RNG).Take(5))
            {
                await target.Effect.Damage(2, source);
            }
        }

        internal static async Task ArmorAndBoostAdjacent(GameCard source)
        {
            await source.Effect.Armor(5, source);
            foreach (var target in source.GetRangeCard(1, GetRangeType.HollowAll)
                .Where(x => x.Status.CardRow.IsOnPlace()).ToList())
            {
                await target.Effect.Boost(1, source);
            }
        }
    }
}
