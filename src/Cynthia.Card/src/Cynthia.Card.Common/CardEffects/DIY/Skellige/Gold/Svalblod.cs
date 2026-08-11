using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70099")]//斯瓦勃洛 Svalblod
    public class Svalblod : CardEffect
    {//对牌组中的所有单位牌造成2点伤害，随后，随后强化2点战力。
        public Svalblod(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deckUnits = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow &&
                            !x.Status.IsSpying &&
                            x != Card)
                .Select(x => new { Card = x, Strength = x.Status.Strength })
                .ToList();
            var handUnits = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow &&
                            !x.Status.IsSpying &&
                            x != Card)
                .Select(x => new { Card = x, Strength = x.Status.Strength })
                .ToList();

            foreach (var entry in deckUnits.Where(x => x.Strength >= 2))
            {
                await entry.Card.Effect.Damage(2, Card);
                await entry.Card.Effect.Strengthen(2, Card);
            }

            foreach (var entry in handUnits.Where(x => x.Strength >= 2))
            {
                // The generic damage pipeline deliberately leaves a hand unit at
                // one point. Svalblod is the explicit exception: the following
                // strengthen keeps the card alive after taking the full damage.
                await Game.ShowCardNumberChange(entry.Card, -2, NumberType.Normal);
                entry.Card.Status.HealthStatus -= 2;
                await Game.ShowSetCard(entry.Card);
                await Game.SetPointInfo();
                await Game.ShowCardNumberChange(entry.Card, 2, NumberType.White);
                entry.Card.Status.Strength += 2;
                await Game.ShowSetCard(entry.Card);
                await Game.SetPointInfo();
                await Game.SendEvent(new AfterCardStrengthen(entry.Card, 2, Card));
            }

            foreach (var entry in deckUnits.Where(x => x.Strength <= 2))
            {
                if (entry.Card.Status.CardRow.IsInDeck())
                {
                    await Game.ShowCardMove(
                        new CardLocation(RowPosition.MyCemetery, 0),
                        entry.Card);
                }
            }

            return 0;
        }
    }
}
