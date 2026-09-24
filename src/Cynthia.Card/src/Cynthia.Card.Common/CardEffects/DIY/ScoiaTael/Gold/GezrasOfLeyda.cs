using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId(CardId.GezrasOfLeyda)]
    public class GezrasOfLeyda : Choose
    {
        public GezrasOfLeyda(GameCard card) : base(card) { }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            var selected = await Game.GetSelectPlaceCards(
                Card,
                filter: x => x.IsAnyGroup(Group.Copper, Group.Silver) &&
                             x.Is(type: CardType.Unit) &&
                             x.HasAnyCategorie(Categorie.Witcher, Categorie.Beast),
                selectMode: SelectModeType.MyRow);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }

            if (switchCard == 1)
            {
                await target.Effect.Strengthen(1, Card);
            }
            else if (target.Status.Strength > 1)
            {
                await target.Effect.Weaken(target.Status.Strength - 1, Card);
            }

            target.Effect.Repair(true);
            await target.MoveToCardStayFirst();
            return 1;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>
            {
                { 1, "GezrasOfLeyda_1_Strengthen" },
                { 2, "GezrasOfLeyda_2_Weaken" }
            };
        }
    }
}
