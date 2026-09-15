using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70150")]//文森特·凡·莫拉汉姆 VincentvanMoorlehem
    public class VincentvanMoorlehem : CardEffect
    {//
        public VincentvanMoorlehem(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var inspected = Game.PlayersDeck[AnotherPlayer]
                .Where(x => x.IsAnyGroup(Group.Copper, Group.Silver) &&
                            x.Is(type: CardType.Unit) &&
                            x.CardPoint() > 1 &&
                            !x.Status.IsSpying &&
                            x.CardInfo().CardUseInfo != CardUseInfo.EnemyRow &&
                            x.CardInfo().CardUseInfo != CardUseInfo.EnemyPlace)
                .Mess(RNG)
                .Take(3)
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, inspected, isEnemyBack: false))
                .TrySingle(out var deckUnit))
            {
                return 0;
            }

            var before = deckUnit.CardPoint();
            await deckUnit.Effect.Lower_Power_By(before - 1, Card);
            var lostPower = deckUnit.Status.CardRow.IsInDeck()
                ? Math.Max(0, before - Math.Max(0, deckUnit.CardPoint()))
                : Math.Max(0, before);
            if (lostPower <= 0)
            {
                return 0;
            }

            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (selectList.TrySingle(out var target))
            {
                await target.Effect.Damage(lostPower, Card);
            }
            return 0;
        }
    }
}
