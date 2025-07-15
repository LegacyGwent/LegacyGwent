using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70178")]//西格瓦尔德
    public class Ulle : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterTurnStart>
    {//On turn start, if this card is in the graveyard, resurrect self. On turn end, duel the lowest enemy, if you win, banish self.
    // 回合开始时，复活自身到随机排，回合结束时，与最弱的敌方单位决斗，如果获胜，则放逐自身。
        public Ulle(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        // On turn start, if this card is in the graveyard, resurrect self.
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            return;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        // On turn end, duel the lowest enemy, if you win, banish self.
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if (!Game.GetPlaceCards(AnotherPlayer).WhereAllLowest().TryMessOne(out var target, Game.RNG))
            {
                return;
            }
            await Duel(target, Card);
            if (Card.IsDead || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            Card.Status.IsDoomed = true;
            return;
        }
    }
}
