using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70098")] //维里赫德旅破坏者 VriheddSaboteur
    public class VriheddSaboteur : Choose
    {//Play a random item from your deck or choose and play a scoiatael item from your deck. Boost self by 3 if the item kills a card
        public VriheddSaboteur(GameCard card) : base(card){}
        // private bool isneedboost = false;
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
                {1, "VriheddSaboteur_1_RandomItem"},
                {2, "VriheddSaboteur_2_STItem"}
            };
        }
        // play a random item from your deck
        private async Task<int> FUNCTION1()
        { 
            var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special)).ToList();
            if (list.Count() == 0) return 0;
            var item = list.Mess(RNG).First();
            await item.MoveToCardStayFirst();
            return 1;
        }
            // choose and play a scoiatael item from your deck
        private async Task<int> FUNCTION2()
        {
            var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special && x.CardInfo().Faction == Faction.ScoiaTael)).ToList();
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "");
            if (result.Count() == 0) return 0;
            var item = result.First();
            await item.MoveToCardStayFirst();
            return 1;
        }
    }
}