using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70137")]//巨橡 TheGreatOak
    public class TheGreatOak : Choose
    {
        private GameCard DryadTarget = null;
        public TheGreatOak(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await FUNCTION1();
                case 2:
                    return await FUNCTION2();
            }

            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "TheGreatOak_1_Weaken"},
                {2, "TheGreatOak_2_Resurect"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var damageList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!damageList.TrySingle(out var Dtarget))
            {
                return 0;
            }
            await Dtarget.Effect.Weaken(Dtarget.Status.Strength / 2, Card);
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Dryad));
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1);
            if (result.Count() == 0) return 0;
            DryadTarget = result.First();
            await DryadTarget.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (DryadTarget == null)
            {
                return;
            }
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count)), DryadTarget);
            return;
        }
    }
}
