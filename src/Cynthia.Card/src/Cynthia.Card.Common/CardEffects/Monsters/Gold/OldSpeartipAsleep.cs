using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22001")]//老矛头：昏睡
    public class OldSpeartipAsleep : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardTransform>
    {// Deploy: Boost adjacent units by 1. Gain 5 Armor. At the start of your turn, if there are at least 3 enemy units on the opposite row, transform into Old Speartip: Awake.
        //使相邻单位获得1点增益，自身获得5点护甲。己方回合开始时，若对方同排有至少3个单位，则苏醒
        public OldSpeartipAsleep(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ArmorAndBoost();
            return 0;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {// if there are at least 3 enemy units on the opposite row, transform into Old Speartip: Awake
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            if (@event.PlayerIndex != PlayerIndex)
            {
                var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
                if (row.Count() >= 3)
                {
                    await Card.Effect.Transform(CardId.OldSpeartip, Card);
                }
            }
        }
        public async Task HandleEvent(AfterCardTransform @event)
        {
            if (@event.Target != Card || !Card.IsAliveOnPlance() || @event.Source.Status.CardId != CardId.OldSpeartip)
            {
                return;
            }
            await ArmorAndBoost();
            return;
        }

        private async Task ArmorAndBoost()
        {
            await Card.Effect.Armor(5, Card);
            var list = Card.GetRangeCard(1, type: GetRangeType.HollowAll).ToList();
            if (list.Count() == 0)
            {
                return;
            }
            foreach (var card in list)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    await card.Effect.Boost(1, Card);
                }
            }
            return;
        }
    }
}