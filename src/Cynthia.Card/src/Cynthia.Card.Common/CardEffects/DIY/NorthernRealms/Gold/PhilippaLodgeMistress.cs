using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70086")]//艾勒的格哈特
    public class PhilippaLodgeMistress : Choose
    {//play a bronze or silver mage from your deck, or generate and play a bronze spell
        public PhilippaLodgeMistress(GameCard card) : base(card)
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
                {1, "PhilippaLodgeMistress_1_PlayMage"},
                {2, "PhilippaLodgeMistress_2_CreateSpell"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            //select mages from your deck
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Mage) &&
                    (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //choose mage to play
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0)
            {
                return 0;
            }
            var playCard = cards.Single();
            await playCard.MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> FUNCTION2()
        {
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Special, x => x.HasAllCategorie(Categorie.Spell))
                 .Select(x => x.CardId);

            return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
        }
    }
}
