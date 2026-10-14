using System;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70089")]
    public class Artis : CardEffect
    {
        public Artis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!targets.TrySingle(out var target)) return 0;
            var before = target.CardPoint();
            await target.Effect.Damage(9, Card);
            var actualLoss = Math.Max(0, Math.Min(before, target.Status.CardRow.IsOnPlace()
                ? before - target.CardPoint() : before));
            await Game.CreateCard(CardId.SvalblodFanatic, PlayerIndex,
                new CardLocation(Card.Status.CardRow, int.MaxValue),
                status => { status.Strength = actualLoss; status.IsDoomed = true; }, source: Card);
            return 0;
        }
    }
}
